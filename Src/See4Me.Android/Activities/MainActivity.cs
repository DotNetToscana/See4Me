using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using See4Me.ViewModels;
using See4Me.Android.Common;
using GalaSoft.MvvmLight.Messaging;
using Android.Media;
using Android.OS;
using Android.Hardware;
using GalaSoft.MvvmLight.Helpers;
using Java.Security;
using Messaging = GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using See4Me.Android.Extensions;
using See4Me.Services;
using Android.Content.Res;
using Android.Content.PM;
using System.Collections.Generic;

namespace See4Me.Android
{
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
         ScreenOrientation = ScreenOrientation.Landscape, Theme = "@style/See4Me")]
    public class MainActivity : ActivityBase<MainViewModel>
    {
        private List<Binding> bindings;

        private TextureView textureView;
        private TextView statusMessage;
        private ImageButton takePhotoButton;

        public TextView StatusMessage => statusMessage ?? (statusMessage = FindViewById<TextView>(Resource.Id.textViewMessage));

        public ImageButton TakePhotoButton => takePhotoButton ?? (takePhotoButton = FindViewById<ImageButton>(Resource.Id.takePhotoButton));

        public TextureView TextureView => textureView ?? (textureView = FindViewById<TextureView>(Resource.Id.textureViewMain));

        protected override int? LayoutResourseID => Resource.Layout.Main;

        protected override void OnInitialize(Bundle bundle)
        {
            this.RegisterMessages();
            this.bindings = new List<Binding>()
            {
                this.SetBinding(() => ViewModel.StatusMessage, () => StatusMessage.Text, BindingMode.OneWay).RegisterHandler(StatusMessage),
            };

            this.TakePhotoButton.SetCommand(this.ViewModel.DescribeImageCommand);

            // Initializes camera streaming.
            var streamingService = ServiceLocator.Current.GetInstance<IStreamingService>() as TextureView.ISurfaceTextureListener;
            TextureView.SurfaceTextureListener = streamingService;

            base.OnInitialize(bundle);
        }

        private void RegisterMessages()
        {
            Messaging.Messenger.Default.Register<NotificationMessageAction<object>>(this, (message) =>
            {
                switch (message.Notification)
                {
                    case Constants.InitializeStreaming:
                        message.Execute(TextureView);
                        break;
                }
            });
        }

        protected override async void OnResume()
        {
            base.OnResume();

            await ViewModel.InitializeStreamingAsync();
            await ViewModel.CheckShowConsentAsync();
        }

        protected override async void OnPause()
        {
            base.OnPause();

            await ViewModel.CleanupAsync();
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.VolumeDown || keyCode == Keycode.VolumeUp)
            {
                ViewModel.DescribeImageCommand.Execute(null);
                return true;
            }

            return base.OnKeyDown(keyCode, e);
        }
    }
}

