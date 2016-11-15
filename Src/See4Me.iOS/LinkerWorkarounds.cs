using System;
using Foundation;
using UIKit;

namespace See4Me
{
	[Preserve]
	static class LinkerWorkarounds
	{
		public static void KeepTheseMethods()
		{
			default(UITextField).Text = "";
			default(UISwitch).Selected = true;
			throw new Exception("Don't actually call this!");
		}
	}
}

