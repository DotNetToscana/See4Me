using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine.Services.ServiceSettings
{
    public interface IVisionSettingsProvider
    {
        Task<VisionSettings> GetSettingsAsync();
    }
}
