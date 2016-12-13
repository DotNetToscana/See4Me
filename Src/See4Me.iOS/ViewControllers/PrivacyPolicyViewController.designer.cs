// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace See4Me
{
	[Register ("PrivacyPolicyViewController")]
	partial class PrivacyPolicyViewController
	{
		[Outlet]
		UIKit.UILabel LabelPageTitle { get; set; }

		[Outlet]
		UIKit.UITextView LabelPrivacyText { get; set; }

		[Outlet]
		UIKit.UILabel LinkHomePageCognitiveServices { get; set; }

		[Outlet]
		UIKit.UILabel LinkPrivacyPolicy { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LabelPageTitle != null) {
				LabelPageTitle.Dispose ();
				LabelPageTitle = null;
			}

			if (LabelPrivacyText != null) {
				LabelPrivacyText.Dispose ();
				LabelPrivacyText = null;
			}

			if (LinkHomePageCognitiveServices != null) {
				LinkHomePageCognitiveServices.Dispose ();
				LinkHomePageCognitiveServices = null;
			}

			if (LinkPrivacyPolicy != null) {
				LinkPrivacyPolicy.Dispose ();
				LinkPrivacyPolicy = null;
			}
		}
	}
}
