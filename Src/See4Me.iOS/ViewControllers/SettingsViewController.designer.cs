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
		UIKit.UILabel LabelFaceSubscriptionKey { get; set; }

		[Outlet]
		UIKit.UILabel LabelShowDescriptionOnFaceIdentification { get; set; }

		[Outlet]
		UIKit.UILabel LabelShowOriginalDescription { get; set; }

		[Outlet]
		UIKit.UILabel LabelShowRecognitionConfidence { get; set; }

		[Outlet]
		UIKit.UILabel LabelTextToSpeech { get; set; }

		[Outlet]
		UIKit.UILabel LabelTranslatorSubscriptionKey { get; set; }

		[Outlet]
		UIKit.UILabel LabelVisionSubscriptionKey { get; set; }

		[Outlet]
		UIKit.UILabel LinkSubscribeFace { get; set; }

		[Outlet]
		UIKit.UILabel LinkSubscribeVision { get; set; }

		[Outlet]
		UIKit.UILabel LinkTranslatorSubscriptionKey { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchShowDescriptionConfidence { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchShowDescriptionOnFaceIdentification { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchShowOriginalDescription { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchTextToSpeech { get; set; }

		[Outlet]
		UIKit.UITextField TextFaceSubscriptionKey { get; set; }

		[Outlet]
		UIKit.UITextField TextTranslatorSubscriptionKey { get; set; }

		[Outlet]
		UIKit.UITextField TextVisionSubscriptionKey { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ButtonAbout != null) {
				ButtonAbout.Dispose ();
				ButtonAbout = null;
			}

			if (ButtonPrivacy != null) {
				ButtonPrivacy.Dispose ();
				ButtonPrivacy = null;
			}

			if (LabelFaceSubscriptionKey != null) {
				LabelFaceSubscriptionKey.Dispose ();
				LabelFaceSubscriptionKey = null;
			}

			if (LabelShowDescriptionOnFaceIdentification != null) {
				LabelShowDescriptionOnFaceIdentification.Dispose ();
				LabelShowDescriptionOnFaceIdentification = null;
			}

			if (LabelShowOriginalDescription != null) {
				LabelShowOriginalDescription.Dispose ();
				LabelShowOriginalDescription = null;
			}

			if (LabelShowRecognitionConfidence != null) {
				LabelShowRecognitionConfidence.Dispose ();
				LabelShowRecognitionConfidence = null;
			}

			if (LabelTextToSpeech != null) {
				LabelTextToSpeech.Dispose ();
				LabelTextToSpeech = null;
			}

			if (LabelTranslatorSubscriptionKey != null) {
				LabelTranslatorSubscriptionKey.Dispose ();
				LabelTranslatorSubscriptionKey = null;
			}

			if (LabelVisionSubscriptionKey != null) {
				LabelVisionSubscriptionKey.Dispose ();
				LabelVisionSubscriptionKey = null;
			}

			if (LinkSubscribeVision != null) {
				LinkSubscribeVision.Dispose ();
				LinkSubscribeVision = null;
			}

			if (LinkSubscribeFace != null) {
				LinkSubscribeFace.Dispose ();
				LinkSubscribeFace = null;
			}

			if (LinkTranslatorSubscriptionKey != null) {
				LinkTranslatorSubscriptionKey.Dispose ();
				LinkTranslatorSubscriptionKey = null;
			}

			if (SwitchShowDescriptionConfidence != null) {
				SwitchShowDescriptionConfidence.Dispose ();
				SwitchShowDescriptionConfidence = null;
			}

			if (SwitchShowDescriptionOnFaceIdentification != null) {
				SwitchShowDescriptionOnFaceIdentification.Dispose ();
				SwitchShowDescriptionOnFaceIdentification = null;
			}

			if (SwitchShowOriginalDescription != null) {
				SwitchShowOriginalDescription.Dispose ();
				SwitchShowOriginalDescription = null;
			}

			if (SwitchTextToSpeech != null) {
				SwitchTextToSpeech.Dispose ();
				SwitchTextToSpeech = null;
			}

			if (TextFaceSubscriptionKey != null) {
				TextFaceSubscriptionKey.Dispose ();
				TextFaceSubscriptionKey = null;
			}

			if (TextTranslatorSubscriptionKey != null) {
				TextTranslatorSubscriptionKey.Dispose ();
				TextTranslatorSubscriptionKey = null;
			}

			if (TextVisionSubscriptionKey != null) {
				TextVisionSubscriptionKey.Dispose ();
				TextVisionSubscriptionKey = null;
			}
		}
	}
}
