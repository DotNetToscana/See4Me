// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace See4Me
{
    [Register ("SettingsViewController")]
    partial class SettingsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
		[Preserve]
        UIKit.UITextView EmotionSubscribeLink { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
		[Preserve]
		UIKit.UITextField TextEmotionSubscriptionKey { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
		[Preserve]
		UIKit.UITextField TextVisionSubscriptionKey { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
		[Preserve]
		UIKit.UITextView VisionSubscribeLink { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (EmotionSubscribeLink != null) {
                EmotionSubscribeLink.Dispose ();
                EmotionSubscribeLink = null;
            }

            if (TextEmotionSubscriptionKey != null) {
                TextEmotionSubscriptionKey.Dispose ();
                TextEmotionSubscriptionKey = null;
            }

            if (TextVisionSubscriptionKey != null) {
                TextVisionSubscriptionKey.Dispose ();
                TextVisionSubscriptionKey = null;
            }

            if (VisionSubscribeLink != null) {
                VisionSubscribeLink.Dispose ();
                VisionSubscribeLink = null;
            }
        }
    }
}