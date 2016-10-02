using Newtonsoft.Json;
using See4Me.Engine.Services.ServiceSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services.ServiceSettings
{
    public class LocalDemoSettingsProvider : IDemoSettingsProvider
    {
        private const string DEMO_SETTINGS_FILE = "See4Me.Common.DemoSettings.json";

        private IEnumerable<DemoSettings> settings;

        public Task<IEnumerable<DemoSettings>> GetSettingsAsync()
        {
            if (settings == null)
            {
                using (var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(DEMO_SETTINGS_FILE))
                {
                    using (var reader = new StreamReader(stream))
                        settings = JsonConvert.DeserializeObject<IEnumerable<DemoSettings>>(reader.ReadToEnd());
                }
            }

            return Task.FromResult(settings);
        }
    }
}
