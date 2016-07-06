using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using See4Me.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.IO;
using See4Me.Extensions;
using Windows.System.Profile;

namespace See4Me.Views
{
    public sealed partial class MainPage : Page
    {
        private static string deviceFamily;

        public MainPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
            deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.RegisterMessages();
            base.OnNavigatedTo(e);
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<NotificationMessageAction<object>>(this, (message) =>
            {
                switch (message.Notification)
                {
                    case Constants.InitializeStreaming:
                        message.Execute(video);
                        break;
                }
            });

            Messenger.Default.Register<NotificationMessage>(this, (message) =>
            {
                switch (message.Notification)
                {
                    case Constants.TakingPhoto:
                        previewImage.Source = null;

                        if (deviceFamily != "Windows.Mobile")
                            shutter.Play();

                        break;
                }
            });

            Messenger.Default.Register<NotificationMessage<byte[]>>(this, async (message) =>
            {
                switch (message.Notification)
                {
                    case Constants.PhotoTaken:
                        using (var ms = new MemoryStream(message.Content))
                            previewImage.Source = await ms.AsImageSourceAsync();

                        break;
                }
            });
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Messenger.Default.Unregister(this);
            base.OnNavigatingFrom(e);
        }
    }
}
