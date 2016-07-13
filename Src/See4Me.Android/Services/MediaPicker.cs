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