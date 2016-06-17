using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware;
using Android.Graphics;
using GalaSoft.MvvmLight.Views;
using Hardware = global::Android.Hardware;

namespace See4Me.Services
{
#pragma warning disable 0618

    public class StreamingService : Java.Lang.Object, IStreamingService, TextureView.ISurfaceTextureListener, Hardware.Camera.IPictureCallback
    {
        private Hardware.Camera camera;

        private TextureView textureView;
        private SurfaceTexture surfaceTexture;
        private int width;
        private int height;

        private TaskCompletionSource<Stream> pictureResult;

        public ScenarioState CurrentState { get; private set; }

        public CameraPanel CameraPanel { get; private set; } = CameraPanel.Back;

        public Task InitializeAsync() => Task.FromResult<object>(null);

        public Task StartStreamingAsync(CameraPanel panel, object preview)
        {
            this.textureView = preview as TextureView;

            this.TryStart();
            CurrentState = ScenarioState.Streaming;

            return Task.FromResult<object>(null);
        }

        public Task StopStreamingAsync()
        {
            this.Cleanup();
            return Task.FromResult<object>(null);
        }

        public Task<Stream> GetCurrentFrameAsync()
        {
            // Creates the task the will be used to notify when the picture is actually available.
            pictureResult = new TaskCompletionSource<Stream>();
            camera.TakePicture(null, null, this);
            return pictureResult.Task;
        }

        public void OnPictureTaken(byte[] data, Hardware.Camera camera)
        {
            // Sets the task result.
            var ms = new MemoryStream(data);
            pictureResult.SetResult(ms);

            camera.StartPreview();
        }

        public Task SwapCameraAsync()
        {
            throw new NotImplementedException();
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.surfaceTexture = surface;

            this.TryStart();
            CurrentState = ScenarioState.Streaming;
        }

        private void TryStart()
        {
            if (textureView != null && surfaceTexture != null)
            {
                // If necessary, stops the previously used camera object.
                this.Cleanup();

                camera = Hardware.Camera.Open();

                // set resolution, frame rate, preview format, etc.
                var parameters = camera.GetParameters();
                parameters.SetPictureSize(640, 480);
                camera.SetParameters(parameters);

                textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);

                try
                {
                    camera.SetPreviewTexture(surfaceTexture);
                    camera.StartPreview();
                }
                catch (IOException ex)
                {
                    var msg = ex.Message;
                    CurrentState = ScenarioState.Idle;
                }
            }
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            this.Cleanup();
            return true;
        }

        public Task CleanupAsync()
        {
            this.Cleanup();
            return Task.FromResult<object>(null);
        }

        private void Cleanup()
        {
            if (camera != null)
            {
                camera.SetPreviewTexture(null);
                camera.StopPreview();
                camera.Release();
                camera = null;

                this.surfaceTexture = null;
                this.textureView = null;
                this.pictureResult = null;

                CurrentState = ScenarioState.Idle;
            }
        }

        #region Unused ISurfaceTextureListener methods

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height) { }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface) { }

        #endregion
    }

#pragma warning restore 0618
}