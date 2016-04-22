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
    public class MainActivity : ActivityBase<MainViewModel>, GestureDetector.IOnGestureListener
    {
        private List<Binding> bindings;

        private TextureView textureView;
        private TextView statusMessage;
        private GestureDetector gestureDetector;

        public TextView StatusMessage => statusMessage ?? (statusMessage = FindViewById<TextView>(Resource.Id.textViewMessage));

        public TextureView TextureView => textureView ?? (textureView = FindViewById<TextureView>(Resource.Id.textureViewMain));

        protected override int? LayoutResourseID => Resource.Layout.Main;

        protected override void OnInitialize(Bundle bundle)
        {
            this.RegisterMessages();
            this.bindings = new List<Binding>()
            {
                this.SetBinding(() => ViewModel.StatusMessage, () => StatusMessage.Text, BindingMode.OneWay).RegisterHandler(StatusMessage)
            };

            // Initializes camera streaming.
            var streamingService = ServiceLocator.Current.GetInstance<IStreamingService>() as TextureView.ISurfaceTextureListener;
            TextureView.SurfaceTextureListener = streamingService;

            gestureDetector = new GestureDetector(this);

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

            await ViewModel.InitializeAsync();
        }

        protected override async void OnPause()
        {
            base.OnPause();

            await ViewModel.CleanupAsync();
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            ViewModel.VideoCommand.Execute(null);
            return true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            gestureDetector.OnTouchEvent(e);
            return false;
        }

        #region Unused Gesture methods

        public bool OnDown(MotionEvent e) => false;

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY) => false;

        public void OnLongPress(MotionEvent e) { }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY) => false;

        public void OnShowPress(MotionEvent e) { }

        #endregion
    }
}

