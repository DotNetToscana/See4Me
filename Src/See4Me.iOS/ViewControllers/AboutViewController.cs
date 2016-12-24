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
	public partial class AboutViewController : ViewControllerBase<AboutViewModel>
	{
		private List<Binding> bindings;

		public AboutViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationController.SetNavigationBarHidden(false, false);

			this.SetTranslation();
			this.bindings = new List<Binding>()
			{
				this.SetBinding(() => ViewModel.AppVersion, () => LabelAppVersion.Text, BindingMode.OneWay),
				this.SetBinding(() => ViewModel.ProjectAuthor, () => LabelAuthor.Text, BindingMode.OneWay),

				this.SetBinding(() => ViewModel.BlogUrl, () => LinkBlog.Text, BindingMode.OneWay),
				this.SetBinding(() => ViewModel.LinkedInUrl, () => LinkLinkedin.Text, BindingMode.OneWay),
				this.SetBinding(() => ViewModel.TwitterUrl, () => LinkTwitter.Text, BindingMode.OneWay),
				this.SetBinding(() => ViewModel.CognitiveServicesUrl, () => LinkPoweredBy.Text, BindingMode.OneWay),
			};

			LinkBlog.UserInteractionEnabled = true;
			LinkBlog.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.GotoUrlCommand.Execute(ViewModel.BlogUrl);
			}));
			LinkLinkedin.UserInteractionEnabled = true;
			LinkLinkedin.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.GotoUrlCommand.Execute(ViewModel.LinkedInUrl);
			}));
			LinkTwitter.UserInteractionEnabled = true;
			LinkTwitter.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.GotoUrlCommand.Execute(ViewModel.TwitterUrl);
			}));
			LinkPoweredBy.UserInteractionEnabled = true;
			LinkPoweredBy.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.GotoUrlCommand.Execute(ViewModel.CognitiveServicesUrl);
			}));
			LinkGitHub.UserInteractionEnabled = true;
			LinkGitHub.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.GotoGitHubCommand.Execute(null);
			}));
			LinkPrivacy.UserInteractionEnabled = true;
			LinkPrivacy.AddGestureRecognizer(new UITapGestureRecognizer(() =>
			{
				this.ViewModel.GotoPrivacyPolicyCommand.Execute(null);
			}));
		}

		private void SetTranslation()
		{
			this.NavigationItem.Title = AppResources.About;

			this.LabelAppName.Text = AppResources.AppName;
			this.LabelBlog.Text = AppResources.Blog;
			this.LabelLinkedin.Text = AppResources.LinkedIn;
			this.LabelTwitter.Text = AppResources.Twitter;
			this.LabelPoweredBy.Text = AppResources.PoweredByCognitiveServices;

			this.LinkGitHub.Text = AppResources.GotoGitHub;
			this.LinkPrivacy.Text = AppResources.PrivacyPolicyCommand;
		}
	}
}
