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
	public partial class PrivacyPolicyViewController : ViewControllerBase<PrivacyViewModel>
	{
		private List<Binding> bindings;

		public PrivacyPolicyViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationController.SetNavigationBarHidden(false, false);

			this.SetTranslation();
			this.bindings = new List<Binding>()
			{
			
			};

			LinkHomePageCognitiveServices.UserInteractionEnabled = true;
			LinkHomePageCognitiveServices.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.GotoCognitiveServicesUrlCommand.Execute(null);
			}));
			LinkPrivacyPolicy.UserInteractionEnabled = true;
			LinkPrivacyPolicy.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.GotoMicrosoftPrivacyPoliciesUrlCommand.Execute(null);
			}));
		}

		private void SetTranslation()
		{
			this.NavigationItem.Title = AppResources.PrivacyPolicy;

			this.LabelPrivacyText.Text = AppResources.PrivacyStatement;
			this.LinkHomePageCognitiveServices.Text = AppResources.GotoCognitiveServices;
			this.LinkPrivacyPolicy.Text = AppResources.MicrosoftPrivacyPolicies;
		}
	}
}
