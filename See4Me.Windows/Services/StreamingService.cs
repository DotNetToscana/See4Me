using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.Graphics.Display;
using Windows.Devices.Sensors;
using Windows.Storage.FileProperties;
using Windows.Foundation;

namespace See4Me.Services
{
    public class StreamingService : IStreamingService
    {
        /// <summary>
        /// Holds the current scenario state value.
        /// </summary>
        public ScenarioState CurrentState { get; private set; }

        public CameraPanel CameraPanel { get; private set; }

        /// <summary>
        /// References a MediaCapture instance; is null when not in Streaming state.
        /// </summary>
        private MediaCapture mediaCapture;
        private CaptureElement preview;

        // Information about the camera device
        private bool mirroringPreview;
        private bool externalCamera;

        // Receive notifications about rotation of the device and UI and apply any necessary rotation to the preview stream and UI controls       
        private readonly DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
        private readonly SimpleOrientationSensor orientationSensor = SimpleOrientationSensor.GetDefault();
        private SimpleOrientation deviceOrientation = SimpleOrientation.NotRotated;
        private DisplayOrientations displayOrientation = DisplayOrientations.Portrait;

        // Rotation metadata to apply to the preview stream and recorded videos (MF_MT_VIDEO_ROTATION)
        // Reference: http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh868174.aspx
        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

        // Prevent the screen from sleeping while the camera is running
        private readonly DisplayRequest displayRequest = new DisplayRequest();

        /// <summary>
        /// Cache of properties from the current MediaCapture device which is used for capturing the preview frame.
        /// </summary>
        private VideoEncodingProperties videoProperties;

        /// <summary>
        /// Semaphore to ensure FaceTracking logic only executes one at a time
        /// </summary>
        private SemaphoreSlim frameProcessingSemaphore = new SemaphoreSlim(1);

        public Task InitializeAsync()
        {
            // Attempt to lock page to landscape orientation to prevent the CaptureElement from rotating, as this gives a better experience
            //DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;

            // Populate orientation variables with the current state
            displayOrientation = displayInformation.CurrentOrientation;
            if (orientationSensor != null)
                deviceOrientation = orientationSensor.GetCurrentOrientation();

            this.RegisterEventHandlers();

            return Task.CompletedTask;
        }

        public Task CleanupAsync()
        {
            this.UnregisterEventHandlers();

            // Revert orientation preferences
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;

            return Task.CompletedTask;
        }

        private void RegisterEventHandlers()
        {
            // If there is an orientation sensor present on the device, register for notifications
            if (orientationSensor != null)
                orientationSensor.OrientationChanged += OrientationSensor_OrientationChanged;

            displayInformation.OrientationChanged += DisplayInformation_OrientationChanged;
        }

        private void UnregisterEventHandlers()
        {
            if (orientationSensor != null)
                orientationSensor.OrientationChanged -= OrientationSensor_OrientationChanged;

            displayInformation.OrientationChanged -= DisplayInformation_OrientationChanged;
        }

        /// <summary>
        /// Occurs each time the simple orientation sensor reports a new sensor reading.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="args">The event data.</param>
        private void OrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            if (args.Orientation != SimpleOrientation.Faceup && args.Orientation != SimpleOrientation.Facedown)
            {
                // Only update the current orientation if the device is not parallel to the ground. This allows users to take pictures of documents (FaceUp)
                // or the ceiling (FaceDown) in portrait or landscape, by first holding the device in the desired orientation, and then pointing the camera
                // either up or down, at the desired subject.
                //Note: This assumes that the camera is either facing the same way as the screen, or the opposite way. For devices with cameras mounted
                //      on other panels, this logic should be adjusted.
                deviceOrientation = args.Orientation;
            }
        }

        /// <summary>
        /// This event will fire when the page is rotated, when the DisplayInformation.AutoRotationPreferences value set in the SetupUiAsync() method cannot be not honored.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="args">The event data.</param>
        private async void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            displayOrientation = sender.CurrentOrientation;

            if (CurrentState == ScenarioState.Streaming)
                await SetPreviewRotationAsync();
        }

        /// <summary>
        /// Initializes a new MediaCapture instance and starts the Preview streaming to the CamPreview UI element.
        /// </summary>
        /// <returns>Async Task object returning true if initialization and streaming were successful and false if an exception occurred.</returns>
        private async Task<bool> StartWebcamStreamingAsync(CameraPanel panel, CaptureElement preview)
        {
            if (preview == null)
                return false;

            var successful = true;

            try
            {
                this.preview = preview;
                this.preview.Visibility = Visibility.Collapsed;

                // For this scenario, we only need Video (not microphone) so specify this in the initializer.
                // NOTE: the appxmanifest only declares "webcam" under capabilities and if this is changed to include
                // microphone (default constructor) you must add "microphone" to the manifest or initialization will fail.

                // Attempt to get the back camera if one is available, but use any camera device if not
                var cameraDevice = await this.FindCameraDeviceByPanelAsync(this.ConvertCameraPanelToDevicePanel(panel));

                var settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    VideoDeviceId = cameraDevice.Id
                };

                this.mediaCapture = new MediaCapture();
                await this.mediaCapture.InitializeAsync(settings);
                this.mediaCapture.Failed += this.MediaCapture_CameraStreamFailed;

                // Cache the media properties as we'll need them later.
                var deviceController = this.mediaCapture.VideoDeviceController;
                this.videoProperties = deviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

                // Figure out where the camera is located
                if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                {
                    // No information on the location of the camera, assume it's an external camera, not integrated on the device                    
                    externalCamera = true;
                }
                else
                {
                    // Camera is fixed on the device
                    externalCamera = false;
                }

                // Only mirror the preview if the camera is on the front panel
                mirroringPreview = (cameraDevice.EnclosureLocation?.Panel == Windows.Devices.Enumeration.Panel.Front);
                CameraPanel = this.ConvertDevicePanelToCameraPanel(cameraDevice.EnclosureLocation?.Panel ?? Windows.Devices.Enumeration.Panel.Unknown);

                await this.StartPreviewAsync();

                // Ensure the Semaphore is in the signalled state.
                this.frameProcessingSemaphore.Release();
            }
            catch (UnauthorizedAccessException)
            {
                // If the user has disabled their webcam this exception is thrown; provide a descriptive message to inform the user of this fact.
                successful = false;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                successful = false;
            }

            return successful;
        }

        private async Task StartPreviewAsync()
        {
            // Prevent the device from sleeping while the preview is running
            displayRequest.RequestActive();

            // Immediately start streaming to our CaptureElement UI.
            // NOTE: CaptureElement's Source must be set before streaming is started.
            this.preview.Source = this.mediaCapture;
            this.preview.FlowDirection = mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            await this.mediaCapture.StartPreviewAsync();
            await this.SetPreviewRotationAsync();

            this.preview.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Gets the current orientation of the UI in relation to the device (when AutoRotationPreferences cannot be honored) and applies a corrective rotation to the preview
        /// </summary>
        private async Task SetPreviewRotationAsync()
        {
            // Only need to update the orientation if the camera is mounted on the device
            if (externalCamera)
                return;

            // Calculate which way and how far to rotate the preview
            int rotationDegrees = ConvertDisplayOrientationToDegrees(displayOrientation);

            // The rotation direction needs to be inverted if the preview is being mirrored
            if (mirroringPreview)
                rotationDegrees = (360 - rotationDegrees) % 360;

            // Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering and getting preview frames
            var props = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            props.Properties.Add(RotationKey, rotationDegrees);
            await mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
        }

        /// <summary>
        /// Safely stops webcam streaming (if running) and releases MediaCapture object.
        /// </summary>
        private async void ShutdownWebCam()
        {
            if (this.mediaCapture != null)
            {
                if (this.mediaCapture.CameraStreamState == Windows.Media.Devices.CameraStreamState.Streaming)
                {
                    try
                    {
                        await this.mediaCapture.StopPreviewAsync();

                        // Allow the device screen to sleep now that the preview is stopped
                        displayRequest.RequestRelease();
                    }
                    catch
                    {
                        // Since we're going to destroy the MediaCapture object there's nothing to do here
                    }
                }

                this.mediaCapture.Failed -= this.MediaCapture_CameraStreamFailed;
                this.mediaCapture.Dispose();
                this.preview.Source = null;
                this.mediaCapture = null;
            }
        }

        /// <summary>
        /// Handles MediaCapture stream failures by shutting down streaming and returning to Idle state.
        /// </summary>
        /// <param name="sender">The source of the event, i.e. our MediaCapture object</param>
        /// <param name="args">Event data</param>
        private async void MediaCapture_CameraStreamFailed(MediaCapture sender, object args)
        {
            var dispatcher = CoreApplication.MainView?.CoreWindow?.Dispatcher;
            if (dispatcher != null)
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await ChangeScenarioStateAsync(ScenarioState.Idle));
            else
                await ChangeScenarioStateAsync(ScenarioState.Idle);
        }

        /// <summary>
        /// Manages the scenario's internal state. Invokes the internal methods and updates the UI according to the
        /// passed in state value. Handles failures and resets the state if necessary.
        /// </summary>
        /// <param name="newState">State to switch to</param>
        private async Task ChangeScenarioStateAsync(ScenarioState newState, CameraPanel panel = CameraPanel.Back, CaptureElement preview = null)
        {
            switch (newState)
            {
                case ScenarioState.Idle:
                    this.ShutdownWebCam();
                    break;

                case ScenarioState.Streaming:
                    preview = this.preview ?? preview;
                    if (!await this.StartWebcamStreamingAsync(panel, preview))
                    {
                        await this.ChangeScenarioStateAsync(ScenarioState.Idle);
                        break;
                    }

                    break;
            }

            this.CurrentState = newState;
        }

        public async Task StartStreamingAsync(CameraPanel panel, object preview)
        {
            if (CurrentState == ScenarioState.Idle)
                await this.ChangeScenarioStateAsync(ScenarioState.Streaming, panel, preview as CaptureElement);
        }

        public async Task StopStreamingAsync()
        {
            if (CurrentState == ScenarioState.Streaming)
                await this.ChangeScenarioStateAsync(ScenarioState.Idle);
        }

        public async Task SwapCameraAsync()
        {
            try
            {
                await mediaCapture.StopPreviewAsync();
                mediaCapture.Dispose();
                mediaCapture = null;                
            }
            catch { }

            var newPanel = CameraPanel == CameraPanel.Back ? CameraPanel.Front : CameraPanel.Back;
            await this.ChangeScenarioStateAsync(ScenarioState.Streaming, newPanel, preview);
        }

        public async Task<Stream> GetCurrentFrameAsync()
        {
            if (CurrentState != ScenarioState.Streaming)
                return null;

            // If a lock is being held it means we're still waiting for processing work on the previous frame to complete.
            // In this situation, don't wait on the semaphore but exit immediately.
            if (!frameProcessingSemaphore.Wait(0))
                return null;

            try
            {
                // Create a VideoFrame object specifying the pixel format we want our capture image to be.
                // GetPreviewFrame will convert the native webcam frame into this format.
                const BitmapPixelFormat InputPixelFormat = BitmapPixelFormat.Bgra8;
                using (var previewFrame = new VideoFrame(InputPixelFormat, 320, 240))   // (int)this.videoProperties.Width, (int)this.videoProperties.Height)
                {
                    await this.mediaCapture.GetPreviewFrameAsync(previewFrame);

                    // Create a WritableBitmap for our visualization display; copy the original bitmap pixels to wb's buffer.
                    using (var convertedSource = SoftwareBitmap.Convert(previewFrame.SoftwareBitmap, InputPixelFormat))
                    {
                        var displaySource = new WriteableBitmap(convertedSource.PixelWidth, convertedSource.PixelHeight);
                        convertedSource.CopyToBuffer(displaySource.PixelBuffer);

                        return await this.ConvertToStreamAsync(displaySource);
                    }
                }
            }
            catch
            { }
            finally
            {
                frameProcessingSemaphore.Release();
            }

            return null;
        }

        private async Task<Stream> ConvertToStreamAsync(WriteableBitmap bitmap)
        {
            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

            // Get pixels of the WriteableBitmap object 
            var pixelStream = bitmap.PixelBuffer.AsStream();
            var pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            // Save the image file with jpg extension 
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96.0, 96.0, pixels);

            var photoOrientation = this.ConvertOrientationToPhotoOrientation(this.GetCameraOrientation());
            var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };
            await encoder.BitmapProperties.SetPropertiesAsync(properties);

            await encoder.FlushAsync();

            stream.Seek(0);
            return stream.AsStreamForRead();
        }

        /// <summary>
        /// Attempts to find and return a device mounted on the panel specified, and on failure to find one it will return the first device listed
        /// </summary>
        /// <param name="desiredPanel">The desired panel on which the returned device should be mounted, if available</param>
        /// <returns></returns>
        private async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            var desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation?.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }

        #region Panel helpers

        private Windows.Devices.Enumeration.Panel ConvertCameraPanelToDevicePanel(CameraPanel panel)
        {
            switch (panel)
            {
                case CameraPanel.Front:
                    return Windows.Devices.Enumeration.Panel.Front;

                case CameraPanel.Back:
                case CameraPanel.Unknown:
                default:
                    return Windows.Devices.Enumeration.Panel.Back;
            }
        }

        private CameraPanel ConvertDevicePanelToCameraPanel(Windows.Devices.Enumeration.Panel panel)
        {
            switch (panel)
            {
                case Windows.Devices.Enumeration.Panel.Front:
                    return CameraPanel.Front;

                case Windows.Devices.Enumeration.Panel.Back:
                case Windows.Devices.Enumeration.Panel.Unknown:
                default:
                    return CameraPanel.Back;
            }
        }

        #endregion

        #region Rotation helpers

        /// <summary>
        /// Calculates the current camera orientation from the device orientation by taking into account whether the camera is external or facing the user
        /// </summary>
        /// <returns>The camera orientation in space, with an inverted rotation in the case the camera is mounted on the device and is facing the user</returns>
        private SimpleOrientation GetCameraOrientation()
        {
            if (externalCamera)
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            var result = deviceOrientation;

            // Account for the fact that, on portrait-first devices, the camera sensor is mounted at a 90 degree offset to the native orientation
            if (displayInformation.NativeOrientation == DisplayOrientations.Portrait)
            {
                switch (result)
                {
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                        result = SimpleOrientation.NotRotated;
                        break;

                    case SimpleOrientation.Rotated180DegreesCounterclockwise:
                        result = SimpleOrientation.Rotated90DegreesCounterclockwise;
                        break;

                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        result = SimpleOrientation.Rotated180DegreesCounterclockwise;
                        break;

                    case SimpleOrientation.NotRotated:
                        result = SimpleOrientation.Rotated270DegreesCounterclockwise;
                        break;
                }
            }

            // If the preview is being mirrored for a front-facing camera, then the rotation should be inverted
            if (mirroringPreview)
            {
                // This only affects the 90 and 270 degree cases, because rotating 0 and 180 degrees is the same clockwise and counter-clockwise
                switch (result)
                {
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                        return SimpleOrientation.Rotated270DegreesCounterclockwise;

                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        return SimpleOrientation.Rotated90DegreesCounterclockwise;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the given orientation of the device in space to the metadata that can be added to captured photos
        /// </summary>
        /// <param name="orientation">The orientation of the device in space</param>
        /// <returns></returns>
        private PhotoOrientation ConvertOrientationToPhotoOrientation(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return PhotoOrientation.Rotate90;

                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return PhotoOrientation.Rotate180;

                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return PhotoOrientation.Rotate270;

                case SimpleOrientation.NotRotated:
                default:
                    return PhotoOrientation.Normal;
            }
        }

        /// <summary>
        /// Converts the given orientation of the app on the screen to the corresponding rotation in degrees
        /// </summary>
        /// <param name="orientation">The orientation of the app on the screen</param>
        /// <returns>An orientation in degrees</returns>
        private int ConvertDisplayOrientationToDegrees(DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                    return 90;

                case DisplayOrientations.LandscapeFlipped:
                    return 180;

                case DisplayOrientations.PortraitFlipped:
                    return 270;

                case DisplayOrientations.Landscape:
                default:
                    return 0;
            }
        }

        #endregion Rotation helpers
    }
}
