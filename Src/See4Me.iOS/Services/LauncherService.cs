using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace See4Me.Services
{
    public class LauncherService : ILauncherService
    {
        public Task LaunchUriAsync(string uri)
        {
            UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(uri));
            return Task.FromResult<object>(null);
        }

        public Task LaunchMailAsync(string mailAddress) => this.LaunchUriAsync($"mailto:{mailAddress}");

    }
}
