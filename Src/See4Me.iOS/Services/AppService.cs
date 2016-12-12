using Foundation;
using See4Me.Localization.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public class AppService : IAppService
    {
        private const string BLOG_URL = "http://www.riccardocappello.com";
        private const string TWITTER_URL = "https://twitter.com/rcappello";
        private const string LINKEDIN_URL = "https://www.linkedin.com/in/rcappello";

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

        public string Author => AppResources.iOSProjectAuthor;

        public string BlogUrl => BLOG_URL;

        public string TwitterUrl => TWITTER_URL;

        public string LinkedInUrl => LINKEDIN_URL;
    }
}
