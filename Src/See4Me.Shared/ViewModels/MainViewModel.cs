using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Vision;
using See4Me.Common;
using See4Me.Localization.Resources;
using See4Me.Services;
using See4Me.Services.Translator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace See4Me.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IStreamingService streamingService;
        private readonly VisionServiceClient visionServiceClient;
        private readonly ITranslatorService translatorService;
        private readonly ISpeechService speechService;

        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { this.Set(ref statusMessage, value); }
        }

        public RelayCommand VideoCommand { get; set; }

        public RelayCommand<SwipeDirection> SwipeCommand { get; set; }

        private CameraPanel lastCameraPanel = CameraPanel.Unknown;

        public MainViewModel(IStreamingService streamingService, VisionServiceClient visionServiceClient, ITranslatorService translatorService,
            ISpeechService speechService)
        {
            this.streamingService = streamingService;
            this.visionServiceClient = visionServiceClient;
            this.translatorService = translatorService;
            this.speechService = speechService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            VideoCommand = new RelayCommand(async () => await DescribeImageAsync(), () => !IsBusy);
            SwipeCommand = new RelayCommand<SwipeDirection>(async (direction) => await SwapCameraAsync(direction));
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Retrieves tha authorization token for the translator service.
                var task = translatorService.InitializeAsync();

                // Asks the view the UI element in which to start camera streaming.
                Messenger.Default.Send(new NotificationMessageAction<object>(Constants.InitializeStreaming, async (video) =>
                {
                    var successful = false;

                    try
                    {
                        await streamingService.InitializeAsync();
                        await streamingService.StartStreamingAsync(Settings.CameraPanel, video);

                        successful = (streamingService.CurrentState == ScenarioState.Streaming);
                    }
                    catch
                    { }
                    finally
                    {
                        if (successful)
                            await this.NotifyCameraPanelAsync();
                        else
                            await this.NotifyInitializationErrorAsync();
                    }
                }));
            }
            catch
            {
                await this.NotifyInitializationErrorAsync();
            }
        }

        public async Task CleanupAsync()
        {
            try
            {
                await streamingService.StopStreamingAsync();
                await streamingService.CleanupAsync();
            }
            catch { }
        }

        public async Task DescribeImageAsync()
        {
            IsBusy = true;
            string message = null;

            if (IsOnline)
            {
                try
                {
                    MessengerInstance.Send(new NotificationMessage(Constants.PhotoTaken));

                    using (var stream = await streamingService.GetCurrentFrameAsync())
                    {
                        if (stream != null)
                        {
                            StatusMessage = AppResources.QueryingVisionService;

                            var result = await visionServiceClient.DescribeAsync(stream);

                            StatusMessage = AppResources.VisionServiceQueried;

                            if (result.Description.Captions.Length > 0)
                            {
                                message = result.Description.Captions.First().Text;
                                StatusMessage = message;

                                if (Settings.AutomaticTranslation && Language != Constants.DefaultLanguge)
                                {
                                    // The description needs to be translated.
                                    StatusMessage = AppResources.Translating;
                                    message = await translatorService.TranslateAsync(message);
                                }
                            }
                            else
                            {
                                message = AppResources.RecognitionFailed;
                            }
                        }
                        else
                        {
                            message = AppResources.UnableToGetImage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    Debug.WriteLine(msg);

                    message = AppResources.RecognitionError;
                }
            }
            else
            {
                // Internet isn't available, to service cannot be reached.
                message = AppResources.NoConnection;
            }

            // Speaks the result.
            StatusMessage = message;
            await speechService.SpeechAsync(message, languge: Language);

            IsBusy = false;
        }

        public async Task SwapCameraAsync(SwipeDirection direction)
        {
            var successful = false;

            try
            {
                await streamingService.SwapCameraAsync();
                Settings.CameraPanel = streamingService.CameraPanel;

                successful = (streamingService.CurrentState == ScenarioState.Streaming);
            }
            catch
            { }
            finally
            {
                if (successful)
                    await this.NotifyCameraPanelAsync();
                else
                    await this.NotifyInitializationErrorAsync();
            }
        }

        private async Task NotifyCameraPanelAsync()
        {
            // Avoids to notify if the camera panel is the same.
            if (streamingService.CameraPanel != lastCameraPanel)
            {
                var message = streamingService.CameraPanel == CameraPanel.Front ? AppResources.FrontCameraReady : AppResources.BackCameraReady;
                StatusMessage = message;

                await speechService.SpeechAsync(message);

                lastCameraPanel = streamingService.CameraPanel;
            }
        }

        private async Task NotifyInitializationErrorAsync()
        {
            StatusMessage = AppResources.InitializationError;
            await speechService.SpeechAsync(StatusMessage);
        }
    }
}
