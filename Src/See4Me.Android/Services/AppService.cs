using See4Me.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public class AppService : IAppService
    {
        public string Version
        {
            get
            {
                var context = Application.Context;
                var versionName = context.PackageManager.GetPackageInfo(context.PackageName, global::Android.Content.PM.PackageInfoFlags.MetaData).VersionName;
                return versionName;
            }
        }
    }
}
