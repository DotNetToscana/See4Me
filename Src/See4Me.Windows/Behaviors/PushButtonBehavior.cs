using Microsoft.Xaml.Interactivity;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Devices.Gpio;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace See4Me.Behaviors
{
    /// <summary>
    /// A behavior that listens for a specified event on its source and executes its actions when that event is fired.
    /// </summary>
    public sealed class PushButtonBehavior : Behavior
    {
        /// <summary>
        /// Get/Sets the direction of the PushButton pin number
        /// </summary>
        public int PinNumber { get; set; }

        public ButtonType ButtonType { get; set; }

        public PushButton Button { get; private set; }

        public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.Register("ClickCommand", typeof(ICommand),
            typeof(PushButtonBehavior), new PropertyMetadata(null, OnClickCommandPropertyChanged));

        public static void SetClickCommand(DependencyObject d, ICommand value) => d.SetValue(ClickCommandProperty, value);

        public static ICommand GetClickCommand(DependencyObject d) => (ICommand)d.GetValue(ClickCommandProperty);

        public static readonly DependencyProperty LongClickCommandProperty = DependencyProperty.Register("LongClickCommand", typeof(ICommand),
            typeof(PushButtonBehavior), new PropertyMetadata(null, OnLongClickCommandPropertyChanged));

        public static void SetLongClickCommand(DependencyObject d, ICommand value) => d.SetValue(LongClickCommandProperty, value);

        public static ICommand GetLongClickCommand(DependencyObject d) => (ICommand)d.GetValue(LongClickCommandProperty);

        [PlatformSpecific]
        private static void OnClickCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isTypePresent = (GpioController.GetDefault() != null);
            if (isTypePresent)
            {
                var control = d as PushButtonBehavior;
                if (control != null)
                {
                    control.Button.Click += Button_Click;
                }
                else
                {
                    control.Button.Click -= Button_Click;
                }
            }
        }

        [PlatformSpecific]
        private static void OnLongClickCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isTypePresent = (GpioController.GetDefault() != null);
            if (isTypePresent)
            {
                var control = d as PushButtonBehavior;
                if (control != null)
                {
                    control.Button.LongClick += Button_LongClick;
                }
                else
                {
                    control.Button.LongClick -= Button_LongClick;
                }
            }
        }

        [PlatformSpecific]
        private readonly bool isTypePresent;

        [PlatformSpecific]
        public PushButtonBehavior()
        {
            isTypePresent = (GpioController.GetDefault() != null);
        }

        /// <summary>
        /// Called after the behavior is attached to the <see cref="Microsoft.Xaml.Interactivity.Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (isTypePresent)
            {
                Button = new PushButton(PinNumber, ButtonType, this);
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="Microsoft.Xaml.Interactivity.Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (isTypePresent)
            {
                Button.Click -= Button_Click;
                Button.LongClick -= Button_LongClick;
            }
        }

        private static void Button_Click(object sender, EventArgs e)
        {
            var control = (sender as PushButton).Behavior;
            GetClickCommand(control)?.Execute(null);
        }

        private static void Button_LongClick(object sender, EventArgs e)
        {
            var control = (sender as PushButton).Behavior;
            GetLongClickCommand(control)?.Execute(null);
        }
    }
}
