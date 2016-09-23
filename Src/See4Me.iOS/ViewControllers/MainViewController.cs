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

		public UITextView MessageText { get; set; }

		public UIButton TakePhotoButton { get; set; }
		public UIButton SwapCameraButton { get; set; }

		public UIImageView PreviewImage { get; set; }
		private bool PreviewImageCollapsed = false;

		public MainViewController(IntPtr handle) : base(handle)
        { }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

			CGRect ScreenBounds = UIScreen.MainScreen.Bounds;

			View.Frame = ScreenBounds;
            View.Bounds = ScreenBounds;
            previewLayer = new AVCaptureVideoPreviewLayer
            {
                BackgroundColor = UIColor.LightGray.CGColor,
                MasksToBounds = true,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
				Frame = this.View.Bounds
            };

            View.Layer.AddSublayer(previewLayer);

			//PreivewImage
			PreviewImage = new UIImageView()
			{
				Frame = new RectangleF(0,0,
				                       (float)ScreenBounds.Width / 4, 
				                       (float)View.Frame.Height / 4),
				Alpha = 0.75f,
				UserInteractionEnabled = true
			};

			//TakePhoto button
			TakePhotoButton = new UIButton(UIButtonType.System)
			{
				Frame = new RectangleF((float)ScreenBounds.Width - 70f, 
				                       ((float)View.Frame.Height / 2) - 30f, 
				                       60f, 60f),
				BackgroundColor = UIColor.Black.ColorWithAlpha(0.25f),

			};
			TakePhotoButton.TintColor = UIColor.White;
			TakePhotoButton.SetImage(UIImage.FromFile("Images/Camera.png"), UIControlState.Normal);
			TakePhotoButton.Layer.CornerRadius = 30f;
			TakePhotoButton.SetCommand(this.ViewModel.DescribeImageCommand);

			//SwapCamera button
			SwapCameraButton = new UIButton(UIButtonType.System)
			{
				Frame = new RectangleF((float)ScreenBounds.Width / 2 - 30f,
									   10f,
									   60f, 60f),
				BackgroundColor = UIColor.Black.ColorWithAlpha(0.25f),

			};
			SwapCameraButton.TintColor = UIColor.White;
			SwapCameraButton.SetImage(UIImage.FromFile("Images/SwitchCamera.png"), UIControlState.Normal);
			SwapCameraButton.Layer.CornerRadius = 30f;
			SwapCameraButton.SetCommand(this.ViewModel.SwapCameraCommand);

			//Message Label
			MessageText = new UITextView
            {
                Frame = new RectangleF(40f, (float)View.Frame.Height - 180f, (float)UIScreen.MainScreen.Bounds.Width - 80f, 180f),
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Text = Strings.AppName.Localize(),
				UserInteractionEnabled = false,
				BackgroundColor = UIColor.FromRGBA(255,255,255,0)
            };
            MessageText.Font = MessageText.Font.WithSize(24.0f);

			View.AddSubview(TakePhotoButton);
			View.AddSubview(SwapCameraButton);

			View.AddSubview(PreviewImage);

			View.AddSubview(MessageText);

			// Tap
			PreviewImage.AddGestureRecognizer(new UITapGestureRecognizer(tap => 
			{
				if (PreviewImageCollapsed)
				{
					PreviewImage.Frame = new RectangleF(0, 0,
										   (float)ScreenBounds.Width,
										   (float)View.Frame.Height);
					PreviewImage.Alpha = 1f;
				}
				else 
				{
					PreviewImage.Frame = new RectangleF(0, 0,
										   (float)ScreenBounds.Width / 4,
										   (float)View.Frame.Height / 4);
					PreviewImage.Alpha = 0.75f;
				}
				PreviewImageCollapsed = !PreviewImageCollapsed;
			}));

			this.RegisterMessages();
            this.bindings = new List<Binding>()
            {
                this.SetBinding(() => ViewModel.StatusMessage, () => MessageText.Text, BindingMode.OneWay)
            };

            await ViewModel.InitializeStreamingAsync();
            await ViewModel.CheckShowConsentAsync();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			var previewLayerConnection = previewLayer.Connection;
			if (previewLayerConnection.SupportsVideoOrientation)
				previewLayerConnection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
		}

        public override async void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            await ViewModel.CleanupAsync();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            previewLayer.Frame = this.View.Frame;

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
						PreviewImage.Image = null;
                        SoundTools.TriggerSoundAndViber();
                        break;
                }
            });

			Messenger.Default.Register<NotificationMessage<byte[]>>(this, (message) =>
			{
				switch (message.Notification)
				{
					case Constants.PhotoTaken:
						PreviewImage.Image = null;
						PreviewImage.Image = UIImage.LoadFromData(NSData.FromArray(message.Content)); ;
						break;
				}
			});

			Messenger.Default.Register<PropertyChangedMessageBase>(this, true, (property) =>
			{
				if (property.PropertyName == "StatusMessage")
				{
					var topCorrect = MessageText.Bounds.Height - MessageText.ContentSize.Height;
					topCorrect = (topCorrect < 0.0f ? 0.0f : topCorrect);
					MessageText.ContentOffset = new CGPoint(0, -topCorrect);
				}
			});
        }
    }
}