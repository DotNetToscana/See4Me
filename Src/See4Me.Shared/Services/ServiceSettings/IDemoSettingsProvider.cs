using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services.ServiceSettings
{
    public interface IDemoSettingsProvider
    {
        Task<IEnumerable<DemoSettings>> GetSettingsAsync();
    }
}
