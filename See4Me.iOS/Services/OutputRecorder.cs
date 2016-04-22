using System;
using System.Collections.Generic;
using System.Text;
using AVFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using UIKit;

namespace See4Me.Services
{
    public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        private UIImage image;

        public UIImage GetImage()
        {
            lock (syncObject)
                return image;
        }

        private static object syncObject = new object();

        public OutputRecorder()
        { }

        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            try
            {
                lock (syncObject)
                {
                    this.TryDispose(image);
                    image = ImageFromSampleBuffer(sampleBuffer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                //
                // Although this looks innocent "Oh, he is just optimizing this case away"
                // this is incredibly important to call on this callback, because the AVFoundation
                // has a fixed number of buffers and if it runs out of free buffers, it will stop
                // delivering frames. 
                //  
                sampleBuffer.Dispose();
            }
        }

        private void TryDispose(IDisposable obj)
        {
            try
            {
                if (obj != null)
                    obj.Dispose();
            }
            catch
            { }
        }

        private UIImage ImageFromSampleBuffer(CMSampleBuffer sampleBuffer)
        {
            // Get the CoreVideo image
            using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
            {
                // Lock the base address
                pixelBuffer.Lock(CVOptionFlags.None);

                // Get the number of bytes per row for the pixel buffer
                var baseAddress = pixelBuffer.BaseAddress;
                var bytesPerRow = (int)pixelBuffer.BytesPerRow;
                var width = (int)pixelBuffer.Width;
                var height = (int)pixelBuffer.Height;
                var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;

                // Create a CGImage on the RGB colorspace from the configured parameter above
                using (var cs = CGColorSpace.CreateDeviceRGB())
                {
                    using (var context = new CGBitmapContext(baseAddress, width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo)flags))
                    {
                        using (CGImage cgImage = context.ToImage())
                        {
                            pixelBuffer.Unlock(CVOptionFlags.None);
                            return UIImage.FromImage(cgImage);
                        }
                    }
                }
            }
        }
    }
}
