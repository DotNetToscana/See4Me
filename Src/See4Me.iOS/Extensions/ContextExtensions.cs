using System;
using UIKit;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;

namespace See4Me
{
	public static class ContextExtensions
	{
		public static Binding<TSource, TTarget> RegisterHandler<TSource, TTarget>(this Binding<TSource, TTarget> binding, UIControl control)
		{
			if (control is UITextField)
				(control as UITextField).EditingChanged += (s, e) => { };
			else if (control is UISwitch)
				(control as UISwitch).ValueChanged += (s, e) => { };

			return binding;
		}

		public static UIButton SetCommand(this UIButton control, RelayCommand command)
		{
			control.TouchUpInside += (s, e) => { };

			control.SetCommand(Events.TouchUpInside, command);
			control.Enabled = command.CanExecute(null);
			command.CanExecuteChanged += (s, args) => control.Enabled = command.CanExecute(null);

			return control;
		}

		public static UIBarButtonItem SetCommand(this UIBarButtonItem control, RelayCommand command)
		{
			control.Clicked += (s, e) => { };

			control.SetCommand(Events.Clicked, command);
			control.Enabled = command.CanExecute(null);

			return control;
		}
	}

	public static class Events
	{
		public const string EditingChanged = "EditingChanged";
		public const string ValueChanged = "ValueChanged";
		public const string TouchUpInside = "TouchUpInside";
		public const string Clicked = "Clicked";
	}
}