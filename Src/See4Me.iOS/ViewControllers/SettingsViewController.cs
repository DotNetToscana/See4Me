using System;
using System.Drawing;
using Foundation;
using GalaSoft.MvvmLight.Messaging;
using See4Me.iOS.Common;
using See4Me.ViewModels;
using UIKit;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using GalaSoft.MvvmLight.Helpers;
using See4Me.iOS.Extensions;
using See4Me.Localization.Resources;
using See4Me.Services;

namespace See4Me.iOS
{
    public partial class SettingsViewController : ViewControllerBase<SettingsViewModel>
    {
		private List<Binding> bindings;

		public SettingsViewController (IntPtr handle) : base (handle)
		{ }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.RegisterMessages();
			this.bindings = new List<Binding>()
			{
				//this.SetBinding(() => ViewModel.StatusMessage, () => MessageText.Text, BindingMode.OneWay)
			};
		}

		private void RegisterMessages()
		{

		}
    }
}