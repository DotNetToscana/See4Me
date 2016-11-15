using System;
using Foundation;
using UIKit;

namespace See4Me
{
	public partial class PrivacyPolicyViewController : UIViewController
	{
		public PrivacyPolicyViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationController.SetNavigationBarHidden(false, false);
		}
	}
}
