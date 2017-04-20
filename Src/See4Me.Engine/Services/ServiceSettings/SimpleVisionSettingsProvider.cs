using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine.Services.ServiceSettings
{
    public class SimpleVisionSettingsProvider : IVisionSettingsProvider
    {
        public SimpleVisionSettingsProvider(VisionSettings settings)
        {
            Settings = settings;
        }

        public VisionSettings Settings { get; set; }

        public Task<VisionSettings> GetSettingsAsync() => Task.FromResult(Settings);
    }
}
