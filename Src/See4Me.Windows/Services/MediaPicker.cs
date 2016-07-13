using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;

namespace See4Me.Services
{
    public class MediaPicker : IMediaPicker
    {
        public async Task<Stream> TakePhotoAsync()
        {
            var captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.AllowCropping = true;
            captureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.MediumXga; ;

            var photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo != null)
            {
                var stream = await photo.OpenAsync(FileAccessMode.Read);
                return stream.AsStreamForRead();
            }

            return null;
        }
    }
}
