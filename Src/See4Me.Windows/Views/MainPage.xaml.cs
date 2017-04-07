using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using See4Me.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.IO;
using See4Me.Extensions;
using Windows.System.Profile;
using See4Me.Common;
using Windows.UI.Core;
using System;
using See4Me.ViewModels;

namespace See4Me.Views
{
    public sealed partial class MainPage : Page
    {
        private static string deviceFamily;

        private readonly MainViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
            deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;

            viewModel = DataContext as MainViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            RegisterMessages();

            base.OnNavigatedTo(e);
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<NotificationMessageAction<object>>(this, (message) =>
            {
                try
                {
                    switch (message.Notification)
                    {
                        case Constants.InitializeStreaming:
                            message.Execute(video);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.GetExceptionMessage();
                    viewModel.StatusMessage = error;
                }
            });

            Messenger.Default.Register<NotificationMessage>(this, (message) =>
            {
                try
                {
                    switch (message.Notification)
                    {
                        case Constants.TakingPhoto:
                            fullSizeImage.Source = null;

                            previewImageBorder.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            previewImage.Source = null;

                            if (deviceFamily != Constants.WindowsMobileFamily)
                                shutter.Play();

                            break;
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.GetExceptionMessage();
                    viewModel.StatusMessage = error;
                }
            });

            Messenger.Default.Register<NotificationMessage<byte[]>>(this, async (message) =>
            {
                try
                {
                    switch (message.Notification)
                    {
                        case Constants.PhotoTaken:
                            await previewImage.SetSourceAsync(message.Content);
                            previewImageBorder.Visibility = Windows.UI.Xaml.Visibility.Visible;

                            await fullSizeImage.SetSourceAsync(message.Content);

                            break;
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.GetExceptionMessage();
                    viewModel.StatusMessage = error;
                }
            });
        }       

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            Messenger.Default.Unregister(this);

            base.OnNavigatingFrom(e);
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (fullSizeImage.Opacity == 1)
            {
                // If the full size image is shown, the back button must actually hide it.
                hideFullSizeImageAnimation.Begin();
                e.Handled = true;
            }
        }
    }
}
