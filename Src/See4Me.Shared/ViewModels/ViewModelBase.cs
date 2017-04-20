using Microsoft.Practices.ServiceLocation;
using See4Me.Services;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
using GalaSoft.MvvmLight.Views;

namespace See4Me.ViewModels
{
    public abstract partial class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected ISettingsService Settings { get; }

        protected INetworkService NetworkService { get; }

        protected Services.INavigationService AppNavigationService { get; }

        protected Services.IDialogService DialogService { get; }

        public ViewModelBase()
        {
            Settings = ServiceLocator.Current.GetInstance<ISettingsService>();
            NetworkService = ServiceLocator.Current.GetInstance<INetworkService>();

            AppNavigationService = ServiceLocator.Current.GetInstance<Services.INavigationService>();
            DialogService = ServiceLocator.Current.GetInstance<Services.IDialogService>();

            IsConnected = NetworkService.IsConnected;
            NetworkService.ConnectivityChanged += (s, e) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => IsConnected = NetworkService.IsConnected);
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