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

			//LinkSubscribeVision.SetCommand(this.ViewModel.SubscribeCognitiveServicesCommand);
			//LinkSubscribeEmotion.SetCommand(this.ViewModel.SubscribeCognitiveServicesCommand);
			//LinkCreateAnApp.SetCommand(this.ViewModel.CreateTranslatorAppCommand);
			//LinkActivateTranslatorService.SetCommand(this.ViewModel.ActivateTranslatorServiceCommand);

			this.NavigationItem.SetRightBarButtonItem(
				new UIBarButtonItem(UIBarButtonSystemItem.Save, (sender, args) =>
				{
					// button was clicked
				})
			, true);
		}

	    public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		
			NavigationController.SetNavigationBarHidden(false, true);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
	
			NavigationController.SetNavigationBarHidden(true, false);
		}

		private void RegisterMessages()
		{

		}
	}
}

