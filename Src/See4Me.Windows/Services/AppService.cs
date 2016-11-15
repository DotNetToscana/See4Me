using See4Me.Localization.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace See4Me.Services
{
    public class AppService : IAppService
    {
        private const string BLOG_URL = "https://marcominerva.wordpress.com";
        private const string TWITTER_URL = "https://twitter.com/marcominerva";
        private const string LINKEDIN_URL = "https://www.linkedin.com/in/marcominerva";

        public string Version
        {
            get
            {
                var version = Package.Current.Id.Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        public string Author => AppResources.WindowsProjectAuthor;

        public string BlogUrl => BLOG_URL;

        public string TwitterUrl => TWITTER_URL;

        public string LinkedInUrl => LINKEDIN_URL;
    }
}
