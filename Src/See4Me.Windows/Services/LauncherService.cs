using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace See4Me.Services
{
    public class LauncherService : ILauncherService
    {
        public Task LaunchUriAsync(string uri) => Launcher.LaunchUriAsync(new Uri(uri)).AsTask();

        public Task LaunchMailAsync(string mailAddress) => this.LaunchUriAsync($"mailto:{mailAddress}");
    }
}
