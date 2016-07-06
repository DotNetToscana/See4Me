using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public class AppService : IAppService
    {
        private readonly NSString buildKey;
        private readonly NSString versionKey;

        public AppService()
        {
            buildKey = new NSString("CFBundleVersion");
            versionKey = new NSString("CFBundleShortVersionString");
        }

        public string Version
        {
            get
            {
                try
                {
                    var build = NSBundle.MainBundle.InfoDictionary.ValueForKey(buildKey);
                    var version = NSBundle.MainBundle.InfoDictionary.ValueForKey(versionKey);
                    return $"{version}.{build}";
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}
