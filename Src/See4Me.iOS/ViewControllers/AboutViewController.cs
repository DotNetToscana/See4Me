using System;
using Foundation;
using UIKit;

namespace See4Me
{
	public partial class AboutViewController : UIViewController
	{
		public AboutViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationController.SetNavigationBarHidden(false, false);
		}
	}
}
