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
			};
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

