using System;
using System.Drawing;
using AVFoundation;
using Foundation;
using GalaSoft.MvvmLight.Messaging;
using See4Me.iOS.Common;
using See4Me.ViewModels;
using UIKit;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Helpers;
using See4Me.iOS.Extensions;
using See4Me.Localization.Resources;
using See4Me.Services;
using CoreGraphics;

namespace See4Me
{
	public partial class SettingsViewController : ViewControllerBase<SettingsViewModel>
	{
		private List<Binding> bindings;

		public SettingsViewController(IntPtr handle) : base(handle)
		{ }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ViewModel.Initialize();

			this.RegisterMessages();
			this.SetTranslation();
			this.bindings = new List<Binding>()
			{
				this.SetBinding(() => ViewModel.VisionSubscriptionKey, () => TextVisionSubscriptionKey.Text, BindingMode.TwoWay),
				this.SetBinding(() => ViewModel.EmotionSubscriptionKey, () => TextEmotionSubscriptionKey.Text, BindingMode.TwoWay),
				this.SetBinding(() => ViewModel.TranslatorClientId, () => TextTranslatorClientID.Text, BindingMode.TwoWay),
				this.SetBinding(() => ViewModel.TranslatorClientSecret, () => TextTranslatorClientSecret.Text, BindingMode.TwoWay),

				this.SetBinding(() => ViewModel.IsTextToSpeechEnabled, () => SwitchTextToSpeech.Selected, BindingMode.TwoWay),
				this.SetBinding(() => ViewModel.ShowDescriptionConfidence, () => SwitchShowDescriptionConfidence.Selected, BindingMode.TwoWay),
				this.SetBinding(() => ViewModel.ShowOriginalDescriptionOnTranslation, () => SwitchShowOriginalDescription.Selected, BindingMode.TwoWay),
			};

			TextVisionSubscriptionKey.DismissKeyboardOnReturn();
			TextEmotionSubscriptionKey.DismissKeyboardOnReturn();
			TextTranslatorClientID.DismissKeyboardOnReturn();
			TextTranslatorClientSecret.DismissKeyboardOnReturn();

			ButtonAbout.SetCommand(this.ViewModel.GotoAboutCommand);
			ButtonPrivacy.SetCommand(this.ViewModel.GotoPrivacyPolicyCommand);

			LinkSubscribeEmotion.UserInteractionEnabled = true;
			LinkSubscribeEmotion.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.SubscribeCognitiveServicesCommand.Execute(null);
			}));
			LinkSubscribeVision.UserInteractionEnabled = true;
			LinkSubscribeVision.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.SubscribeCognitiveServicesCommand.Execute(null);
			}));
			LinkCreateAnApp.UserInteractionEnabled = true;
			LinkCreateAnApp.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.CreateTranslatorAppCommand.Execute(null);
			}));
			LinkActivateTranslatorService.UserInteractionEnabled = true;
			LinkActivateTranslatorService.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.ActivateTranslatorServiceCommand.Execute(null);
			}));

			this.NavigationItem.SetRightBarButtonItem(
				new UIBarButtonItem(UIBarButtonSystemItem.Save, (sender, args) =>
				{
					this.ViewModel.Save();
				})
			, true);

			//dismiss the keyboard if the user taps anywhere in the view
			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			g.CancelsTouchesInView = false; //for iOS5
			View.AddGestureRecognizer(g);

			NavigationController.SetNavigationBarHidden(false, false);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			NavigationController.SetNavigationBarHidden(true, false);
		}

		private void RegisterMessages()
		{

		}

		private void SetTranslation()
		{
			this.NavigationItem.Title = Strings.Settings;

			LabelVisionSubscriptionKey.Text = Strings.VisionSubscriptionKey;
			LabelEmotionSubscriptionKey.Text = Strings.EmotionSubscriptionKey;
			LabelTranslatorClientID.Text = Strings.TranslatorClientId;
			LabelTranslatorClientSecret.Text = Strings.TranslatorClientSecret;

			LinkSubscribeEmotion.Text = Strings.SubscribeCognitiveServices;
			LinkSubscribeVision.Text = Strings.SubscribeCognitiveServices;
			LinkCreateAnApp.Text = Strings.CreateTranslatorApp;
			LinkActivateTranslatorService.Text = Strings.ActivateTranslatorService;

			LabelTextToSpeech.Text = Strings.TextToSpeech;
			LabelShowDescriptionConfidence.Text = Strings.ShowDescriptionConfidence;
			LabelShowOriginalDescription.Text = Strings.ShowOriginalDescriptionOnTranslation;

			ButtonAbout.SetTitle(Strings.AboutCommand, UIControlState.Normal);
			ButtonPrivacy.SetTitle(Strings.PrivacyPolicyCommand, UIControlState.Normal);
		}
	}
}

