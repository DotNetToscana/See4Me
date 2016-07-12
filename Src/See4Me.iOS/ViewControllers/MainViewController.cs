using System;
using System.Drawing;
using AVFoundation;
using Foundation;
using GalaSoft.MvvmLight.Messaging;
using See4Me.iOS.Common;
using See4Me.ViewModels;
using UIKit;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using GalaSoft.MvvmLight.Helpers;
using MediaPlayer;
using SceneKit;
using See4Me.iOS.Extensions;
using See4Me.Localization.Resources;
using See4Me.Services;

namespace See4Me.iOS
{
    public partial class MainViewController : ViewControllerBase<MainViewModel>
    {
        private List<Binding> bindings;

        private AVCaptureVideoPreviewLayer previewLayer;

        public UILabel MessageLabel { get; set; }

        public MainViewController(IntPtr handle) : base(handle)
        { }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = UIScreen.MainScreen.Bounds;
            View.Bounds = UIScreen.MainScreen.Bounds;
            previewLayer = new AVCaptureVideoPreviewLayer
            {
                BackgroundColor = UIColor.LightGray.CGColor,
                MasksToBounds = true,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
                Frame = this.View.Bounds
            };

            View.Layer.AddSublayer(previewLayer);

            MessageLabel = new UILabel
            {
                Frame = new RectangleF(40f, (float)View.Frame.Height - 180f, (float)UIScreen.MainScreen.Bounds.Width - 80f, 180f),
                TextAlignment = UITextAlignment.Center,
                BaselineAdjustment = UIBaselineAdjustment.AlignBaselines,
                TextColor = UIColor.White,
                Text = Strings.AppName.Localize(),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3
            };
            MessageLabel.Font = MessageLabel.Font.WithSize(24.0f);

            View.AddSubview(MessageLabel);

            this.RegisterMessages();
            this.bindings = new List<Binding>()
            {
                this.SetBinding(() => ViewModel.StatusMessage, () => MessageLabel.Text, BindingMode.OneWay)
            };

            // Tap
            View.AddGestureRecognizer(new UITapGestureRecognizer(tap => ViewModel.DescribeImageCommand.Execute(null)));

            // Swipe Left & Right
            var swipeLeft = new UISwipeGestureRecognizer(() => ViewModel.SwapCameraCommand.Execute(null))
            {
                Direction = UISwipeGestureRecognizerDirection.Left,
                Enabled = true
            };

            var swipeRight = new UISwipeGestureRecognizer(() => ViewModel.SwapCameraCommand.Execute(null))
            {
                Direction = UISwipeGestureRecognizerDirection.Right,
                Enabled = true
            };

            View.AddGestureRecognizer(swipeLeft);
            View.AddGestureRecognizer(swipeRight);

            await ViewModel.InitializeAsync();
        }

        public override async void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            await ViewModel.CleanupAsync();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            previewLayer.Frame = this.View.Frame;
            MessageLabel.Frame = new RectangleF(40f, (float)View.Frame.Height - 180f, (float)UIScreen.MainScreen.Bounds.Width - 80f, 180f);

            this.View.LayoutSubviews();

            base.DidRotate(fromInterfaceOrientation);
        }

        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            //Get Preview Layer connection
            var previewLayerConnection = previewLayer.Connection;
            if (previewLayerConnection.SupportsVideoOrientation)
            {
                switch (toInterfaceOrientation)
                {
                    case UIInterfaceOrientation.Portrait:
                        previewLayerConnection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
                        break;

                    case UIInterfaceOrientation.LandscapeRight:
                        previewLayerConnection.VideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
                        break;

                    case UIInterfaceOrientation.LandscapeLeft:
                        previewLayerConnection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
                        break;

                    case UIInterfaceOrientation.PortraitUpsideDown:
                        previewLayerConnection.VideoOrientation = AVCaptureVideoOrientation.PortraitUpsideDown;
                        break;

                    default:
                        previewLayerConnection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
                        break;
                }
            }

            base.WillRotate(toInterfaceOrientation, duration);
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<NotificationMessageAction<object>>(this, (message) =>
                {
                    switch (message.Notification)
                    {
                        case Constants.InitializeStreaming:
                            message.Execute(previewLayer);
                            break;
                    }
                });

            Messenger.Default.Register<NotificationMessage>(this, (message) =>
            {
                switch (message.Notification)
                {
                    case Constants.TakingPhoto:
                        SoundTools.TriggerSoundAndViber();
                        break;
                }
            });
        }
    }
}