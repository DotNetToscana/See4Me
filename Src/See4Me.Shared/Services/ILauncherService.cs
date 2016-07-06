using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public interface ILauncherService
    {
        Task LaunchUriAsync(string uri);

        Task LaunchMailAsync(string mailAddress);
    }
}
