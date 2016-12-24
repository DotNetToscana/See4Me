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
        UIKit.UIButton ButtonAbout { get; set; }


        [Outlet]
        UIKit.UIButton ButtonPrivacy { get; set; }


        [Outlet]
        UIKit.UILabel LabelEmotionSubscriptionKey { get; set; }


        [Outlet]
        UIKit.UILabel LabelShowDescriptionConfidence { get; set; }


        [Outlet]
        UIKit.UILabel LabelShowOriginalDescription { get; set; }


        [Outlet]
        UIKit.UILabel LabelTextToSpeech { get; set; }


        [Outlet]
        UIKit.UILabel LabelTranslatorSubscriptionKey { get; set; }


        [Outlet]
        UIKit.UILabel LabelVisionSubscriptionKey { get; set; }


        [Outlet]
        UIKit.UILabel LinkSubscribeEmotion { get; set; }


        [Outlet]
        UIKit.UILabel LinkSubscribeVision { get; set; }


        [Outlet]
        UIKit.UILabel LinkTranslatorSubscriptionKey { get; set; }


        [Outlet]
        UIKit.UISwitch SwitchShowDescriptionConfidence { get; set; }


        [Outlet]
        UIKit.UISwitch SwitchShowOriginalDescription { get; set; }


        [Outlet]
        UIKit.UISwitch SwitchTextToSpeech { get; set; }


        [Outlet]
        UIKit.UITextField TextEmotionSubscriptionKey { get; set; }


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

            if (LabelEmotionSubscriptionKey != null) {
                LabelEmotionSubscriptionKey.Dispose ();
                LabelEmotionSubscriptionKey = null;
            }

            if (LabelShowDescriptionConfidence != null) {
                LabelShowDescriptionConfidence.Dispose ();
                LabelShowDescriptionConfidence = null;
            }

            if (LabelShowOriginalDescription != null) {
                LabelShowOriginalDescription.Dispose ();
                LabelShowOriginalDescription = null;
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

            if (LinkSubscribeEmotion != null) {
                LinkSubscribeEmotion.Dispose ();
                LinkSubscribeEmotion = null;
            }

            if (LinkSubscribeVision != null) {
                LinkSubscribeVision.Dispose ();
                LinkSubscribeVision = null;
            }

            if (LinkTranslatorSubscriptionKey != null) {
                LinkTranslatorSubscriptionKey.Dispose ();
                LinkTranslatorSubscriptionKey = null;
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