using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public interface IAppService
    {
        string Version { get; }

        string Author { get; }

        string BlogUrl { get; }

        string TwitterUrl { get; }

        string LinkedInUrl { get; }
    }
}
