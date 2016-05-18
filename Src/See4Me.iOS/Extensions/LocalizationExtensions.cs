using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using UIKit;

namespace See4Me.iOS.Extensions
{
    public static class LocalizationExtensions
    {
        public static string Localize(this string s) => NSBundle.MainBundle.LocalizedString(s, s);
    }
}
