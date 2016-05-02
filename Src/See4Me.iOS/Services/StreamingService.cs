using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CoreFoundation;
using CoreImage;
using CoreVideo;
using UIKit;

namespace See4Me.Services
{
    public class StreamingService : IStreamingService
    {
        private AVCaptureSession session;
        private AVCaptureVideoPreviewLayer contentLayer;
        private OutputRecorder outputRecorder;
        private DispatchQueue queue;

        public ScenarioState CurrentState { get; private set; }

        public CameraPanel CameraPanel { get; } = CameraPanel.Back;

        public Task InitializeAsync() => Task.FromResult<object>(null);

        public Task StartStreamingAsync(CameraPanel panel, object preview)
        {
            this.contentLayer = preview as AVCaptureVideoPreviewLayer;

            this.TryStart();
            CurrentState = ScenarioState.Streaming;

            return Task.FromResult<object>(null);
        }

        public Task StopStreamingAsync()
        {
            if (session != null && session.Running)
                session.StopRunning();

            return Task.FromResult<object>(null);
        }

        public Task SwapCameraAsync()
        {
            throw new NotImplementedException();
        }

        private void TryStart()
        {
            if (contentLayer != null)
            {
                session = new AVCaptureSession();

                var camera = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);

                var input = AVCaptureDeviceInput.FromDevice(camera);
                session.AddInput(input);

                // create a VideoDataOutput and add it to the sesion
                var settings = new CVPixelBufferAttributes
                {
                    PixelFormatType = CVPixelFormatType.CV32BGRA
                };

                using (var output = new AVCaptureVideoDataOutput { WeakVideoSettings = settings.Dictionary })
                {
                    queue = new DispatchQueue("s4mQueue");
                    outputRecorder = new OutputRecorder();
                    output.SetSampleBufferDelegate(outputRecorder, queue);
                    session.AddOutput(output);
                }

                this.contentLayer.Session = session;

                session.StartRunning();
            }
        }

        public Task CleanupAsync()
        {
            if (session != null)
            {
                // Stops camera service.
                if (session.Running)
                    session.StopRunning();

                contentLayer = null;
                session = null;
                CurrentState = ScenarioState.Idle;
            }

            return Task.FromResult<object>(null);
        }

        public Task<Stream> GetCurrentFrameAsync()
        {
            var image = outputRecorder.GetImage();
            image = ImageTools.MaxResizeImage(image);

            return Task.FromResult(image.AsJPEG(1.0f).AsStream());
        }
    }
}
