using Android.Content;
using See4Me.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public class LauncherService : ILauncherService
    {
        public Task LaunchUriAsync(string uri)
        {
            var intent = new Intent(Intent.ActionView);
            intent.SetData(global::Android.Net.Uri.Parse(uri));
            MainActivity.CurrentActivity.StartActivity(intent);

            return Task.FromResult<object>(null);
        }

        public Task LaunchMailAsync(string mailAddress)
        {
            var intent = new Intent(Intent.ActionSendto);
            intent.SetData(global::Android.Net.Uri.Parse($"mailto:{mailAddress}"));
            MainActivity.CurrentActivity.StartActivity(intent);

            return Task.FromResult<object>(null);
        }
    }
}
