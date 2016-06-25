using Microsoft.Practices.ServiceLocation;
using See4Me.Services;
using GalaSoft.MvvmLight.Threading;

namespace See4Me.ViewModels
{
    public abstract partial class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected ISettingsService Settings { get; }

        protected INetworkService Network { get; }

        public ViewModelBase()
        {
            Settings = ServiceLocator.Current.GetInstance<ISettingsService>();
            Network = ServiceLocator.Current.GetInstance<INetworkService>();

            IsConnected = Network.IsConnected;
            Network.ConnectivityChanged += (s, e) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => IsConnected = Network.IsConnected);
            };
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (this.SetBusy(value) && !isBusy)
                    BusyMessage = null;
            }
        }

        private string busyMessage;
        public string BusyMessage
        {
            get { return busyMessage; }
            set { this.Set(ref busyMessage, value, broadcast: true); }
        }

        public bool SetBusy(bool value, string message = null)
        {
            BusyMessage = message;

            var isSet = this.Set(() => IsBusy, ref isBusy, value, broadcast: true);
            if (isSet)
            {
                OnIsBusyChangedBase();
                OnIsBusyChanged();
            }

            return isSet;
        }

        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (this.Set(ref isConnected, value, broadcast: true))
                {
                    OnNetworkAvailabilityChangedBase();
                    OnNetworkAvailabilityChanged();
                }
            }
        }

        public string Language => ViewModelLocator.GetLanguage();

        protected virtual void OnIsBusyChanged() { }

        protected virtual void OnNetworkAvailabilityChanged() { }

        partial void OnIsBusyChangedBase();

        partial void OnNetworkAvailabilityChangedBase();
    }
}