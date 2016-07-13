using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public class MediaPicker : IMediaPicker
    {
        public Task<Stream> TakePhotoAsync()
        {
            throw new NotImplementedException();
        }
    }
}
