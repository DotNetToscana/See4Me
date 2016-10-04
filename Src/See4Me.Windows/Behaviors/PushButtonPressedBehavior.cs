using Microsoft.Xaml.Interactivity;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
    [ContentProperty(Name = "Actions")]
    public sealed class PushButtonPressedBehavior : Behavior
    {
        /// <summary>
        /// Get/Sets the direction of the PushButton pin number
        /// </summary>
        public int PinNumber { get; set; }

        public ButtonType ButtonType { get; set; }

        /// <summary>
        /// Identifies the <seealso cref="Actions"/> dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
            "Actions",
            typeof(ActionCollection),
            typeof(PushButtonPressedBehavior),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets the collection of actions associated with the behavior. This is a dependency property.
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                var actionCollection = (ActionCollection)this.GetValue(ActionsProperty);
                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();
                    this.SetValue(ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        [PlatformSpecific]
        private readonly bool isTypePresent;

        private PushButton button;

        [PlatformSpecific]
        public PushButtonPressedBehavior()
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
                button = new PushButton(PinNumber, ButtonType);
                button.Pressed += Button_Pressed;
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="Microsoft.Xaml.Interactivity.Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (isTypePresent)
                button.Pressed -= Button_Pressed;
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            this.OnEvent(this.AssociatedObject, null);
        }

        private void OnEvent(object sender, object eventArgs)
        {
            Interaction.ExecuteActions(this, this.Actions, eventArgs);
        }
    }

    public enum ButtonType
    {
        PullDown = 0,
        PullUp = 1
    }

    public class PushButton : IDisposable
    {
        public readonly GpioController controller;
        public readonly GpioPin pin;

        private readonly GpioPinValue actualLowPinValue;
        private readonly GpioPinValue actualHighPinValue;

        private GpioPinValue lastPinValue;

        public bool IsPressed { get; private set; } = false;

        public event EventHandler Pressed;
        public event EventHandler Released;
        public event EventHandler Click;

        [PlatformSpecific]
        public PushButton(int pinNumber, ButtonType type = ButtonType.PullDown)
        {
            controller = GpioController.GetDefault();
            pin = controller.OpenPin(pinNumber);

            if (type == ButtonType.PullUp)
            {
                actualHighPinValue = GpioPinValue.High;
                actualLowPinValue = GpioPinValue.Low;
            }
            else
            {
                actualHighPinValue = GpioPinValue.Low;
                actualLowPinValue = GpioPinValue.High;
            }

            pin.Write(actualLowPinValue);
            pin.SetDriveMode(GpioPinDriveMode.Input);

            lastPinValue = actualLowPinValue;

            pin.DebounceTimeout = TimeSpan.FromMilliseconds(20);
            pin.ValueChanged += Pin_ValueChanged;
        }

        [PlatformSpecific]
        private void Pin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            var currentPinValue = pin.Read();

            // If same value of last read, exits.
            if (currentPinValue == lastPinValue)
                return;

            // Checks the pin value.
            if (currentPinValue == actualHighPinValue)
            {
                IsPressed = true;
                this.RaiseEvent(Pressed);
            }
            else if (currentPinValue == actualLowPinValue)
            {
                this.RaiseEvent(Released);
                if (IsPressed)
                    this.RaiseEvent(Click);

                IsPressed = false;
            }

            lastPinValue = currentPinValue;
        }

        [PlatformSpecific]
        public void Dispose()
        {
            pin.ValueChanged -= Pin_ValueChanged;
            pin.Dispose();
        }

        private async void RaiseEvent(EventHandler eventHandler)
        {
            var dispatcher = CoreApplication.MainView?.CoreWindow?.Dispatcher;
            if (dispatcher != null)
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => eventHandler?.Invoke(this, EventArgs.Empty));
            else
                eventHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}
