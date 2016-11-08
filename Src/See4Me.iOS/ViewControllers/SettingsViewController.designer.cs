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
	[Register ("SettingsViewController")]
	partial class SettingsViewController
	{
		[Outlet]
		UIKit.UIButton ButtonAbout { get; set; }

		[Outlet]
		UIKit.UIButton ButtonPrivacy { get; set; }

		[Outlet]
		UIKit.UILabel LabelEmotionSubscriptionKey { get; set; }

		[Outlet]
		UIKit.UILabel LabelTranslatorClientID { get; set; }

		[Outlet]
		UIKit.UILabel LabelTranslatorClientSecret { get; set; }

		[Outlet]
		UIKit.UILabel LabelVisionSubscriptionKey { get; set; }

		[Outlet]
		UIKit.UILabel LinkActivateTranslatorService { get; set; }

		[Outlet]
		UIKit.UILabel LinkCreateAnApp { get; set; }

		[Outlet]
		UIKit.UILabel LinkSubscribeEmotion { get; set; }

		[Outlet]
		UIKit.UILabel LinkSubscribeVision { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchShowDescriptionConfidence { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchShowOriginalDescription { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchTextToSpeech { get; set; }

		[Outlet]
		UIKit.UITextField TextEmotionSubscriptionKey { get; set; }

		[Outlet]
		UIKit.UITextField TextTranslatorClientID { get; set; }

		[Outlet]
		UIKit.UITextField TextTranslatorClientSecret { get; set; }

		[Outlet]
		UIKit.UITextField TextVisionSubscriptionKey { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LabelEmotionSubscriptionKey != null) {
				LabelEmotionSubscriptionKey.Dispose ();
				LabelEmotionSubscriptionKey = null;
			}

			if (LabelTranslatorClientID != null) {
				LabelTranslatorClientID.Dispose ();
				LabelTranslatorClientID = null;
			}

			if (LabelTranslatorClientSecret != null) {
				LabelTranslatorClientSecret.Dispose ();
				LabelTranslatorClientSecret = null;
			}

			if (LabelVisionSubscriptionKey != null) {
				LabelVisionSubscriptionKey.Dispose ();
				LabelVisionSubscriptionKey = null;
			}

			if (SwitchShowDescriptionConfidence != null) {
				SwitchShowDescriptionConfidence.Dispose ();
				SwitchShowDescriptionConfidence = null;
			}

			if (SwitchShowOriginalDescription != null) {
				SwitchShowOriginalDescription.Dispose ();
				SwitchShowOriginalDescription = null;
			}

			if (SwitchTextToSpeech != null) {
				SwitchTextToSpeech.Dispose ();
				SwitchTextToSpeech = null;
			}

			if (TextEmotionSubscriptionKey != null) {
				TextEmotionSubscriptionKey.Dispose ();
				TextEmotionSubscriptionKey = null;
			}

			if (TextTranslatorClientID != null) {
				TextTranslatorClientID.Dispose ();
				TextTranslatorClientID = null;
			}

			if (TextTranslatorClientSecret != null) {
				TextTranslatorClientSecret.Dispose ();
				TextTranslatorClientSecret = null;
			}

			if (TextVisionSubscriptionKey != null) {
				TextVisionSubscriptionKey.Dispose ();
				TextVisionSubscriptionKey = null;
			}

			if (ButtonAbout != null) {
				ButtonAbout.Dispose ();
				ButtonAbout = null;
			}

			if (ButtonPrivacy != null) {
				ButtonPrivacy.Dispose ();
				ButtonPrivacy = null;
			}

			if (LinkSubscribeEmotion != null) {
				LinkSubscribeEmotion.Dispose ();
				LinkSubscribeEmotion = null;
			}

			if (LinkSubscribeVision != null) {
				LinkSubscribeVision.Dispose ();
				LinkSubscribeVision = null;
			}

			if (LinkCreateAnApp != null) {
				LinkCreateAnApp.Dispose ();
				LinkCreateAnApp = null;
			}

			if (LinkActivateTranslatorService != null) {
				LinkActivateTranslatorService.Dispose ();
				LinkActivateTranslatorService = null;
			}
		}
	}
}
