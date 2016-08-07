using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services.ServiceSettings
{
    public class LocalVisionSettingsProvider : IVisionSettingsProvider
    {
        private const string VISION_SETTINGS_FILE = "See4Me.Common.VisionSettings.json";

        private VisionSettings settings;

        public Task<VisionSettings> GetSettingsAsync()
        {
            if (settings == null)
            {
                using (var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(VISION_SETTINGS_FILE))
                {
                    using (var reader = new StreamReader(stream))
                        settings = JsonConvert.DeserializeObject<VisionSettings>(reader.ReadToEnd());
                }
            }

            return Task.FromResult(settings);
        }
    }
}
