using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Gpio;
using Windows.UI.Core;

namespace See4Me.Behaviors
{
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

        public PushButtonBehavior Behavior { get; }

        public event EventHandler Pressed;
        public event EventHandler Released;
        public event EventHandler Click;
        public event EventHandler LongClick;

        private readonly Stopwatch longClickWatch;
        private readonly TimeSpan longClickTimeout = TimeSpan.FromMilliseconds(1500);

        [PlatformSpecific]
        public PushButton(int pinNumber, ButtonType type = ButtonType.PullDown, PushButtonBehavior behavior = null)
        {
            Behavior = behavior;
            longClickWatch = new Stopwatch();

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
                longClickWatch.Restart();

                IsPressed = true;
                this.RaiseEvent(Pressed);
            }
            else if (currentPinValue == actualLowPinValue)
            {
                longClickWatch.Stop();

                this.RaiseEvent(Released);

                if (IsPressed)
                {
                    IsPressed = false;

                    if (longClickWatch.Elapsed > longClickTimeout)
                        this.RaiseEvent(LongClick);
                    else
                        this.RaiseEvent(Click);
                }
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
