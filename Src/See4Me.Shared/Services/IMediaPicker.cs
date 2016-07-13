using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public interface IMediaPicker
    {
        Task<Stream> TakePhotoAsync();
    }
}
