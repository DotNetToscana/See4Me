using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using See4Me.Common;
using See4Me.Localization.Resources;
using See4Me.Services;
using See4Me.Services.Translator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using See4Me.Extensions;
using System.IO;
using System.Text;

namespace See4Me.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IStreamingService streamingService;
        private readonly EmotionServiceClient emotionService;
        private readonly VisionServiceClient visionService;
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

        public MainViewModel(IStreamingService streamingService, VisionServiceClient visionService, EmotionServiceClient emotionService,
            ITranslatorService translatorService, ISpeechService speechService)
        {
            this.streamingService = streamingService;
            this.visionService = visionService;
            this.emotionService = emotionService;
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

            string imageDescription = null;
            string facesRecognizedDescription = null;
            string emotionDescription = null;

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

                            var visualFeatures = new VisualFeature[] { VisualFeature.Description, VisualFeature.Faces };
                            var result = await visionService.AnalyzeImageAsync(stream, visualFeatures);

                            if (result.Description.Captions.Length > 0)
                            {
                                imageDescription = result.Description.Captions.First().Text;

                                if (Settings.AutomaticTranslation && Language != Constants.DefaultLanguge)
                                {
                                    // The description needs to be translated.
                                    StatusMessage = AppResources.Translating;
                                    imageDescription = await translatorService.TranslateAsync(imageDescription);
                                }

                                StatusMessage = imageDescription;

                                try
                                {
                                    // If there is one or more faces, asks the service information about them.
                                    if (result.Faces?.Count() > 0)
                                    {
                                        // Describes how many faces have been recognized.
                                        if (result.Faces.Count() == 1)
                                            facesRecognizedDescription = AppResources.FaceRecognizedSingular;
                                        else
                                            facesRecognizedDescription = string.Format(AppResources.FacesRecognizedPlural, result.Faces.Count());

                                        var imageBytes = await stream.ToArrayAsync();
                                        foreach (var face in result.Faces)
                                        {
                                            using (var ms = new MemoryStream(imageBytes))
                                            {
                                                var emotions = await emotionService.RecognizeAsync(ms, face.FaceRectangle.ToRectangle());
                                                var bestEmotion = emotions.FirstOrDefault()?.Scores.GetBestEmotion();

                                                // Creates the emotion description text to be speeched.
                                                var gender = AppResources.ResourceManager.GetString(face.Gender);
                                                var emotion = AppResources.ResourceManager.GetString(bestEmotion + face.Gender);
                                                emotionDescription += string.Format(AppResources.ResourceManager.GetString(Constants.EmotionMessage + face.Gender) + Constants.SentenceEnd, face.Age, gender, emotion);
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                            else
                            {
                                imageDescription = AppResources.RecognitionFailed;
                            }
                        }
                        else
                        {
                            imageDescription = AppResources.UnableToGetImage;
                        }
                    }
                }
                catch
                {
                    imageDescription = AppResources.RecognitionError;
                }
            }
            else
            {
                // Internet isn't available, to service cannot be reached.
                imageDescription = AppResources.NoConnection;
            }

            // Speaks the result.
            StatusMessage = imageDescription;
            var message = $"{imageDescription}{Constants.SentenceEnd} {facesRecognizedDescription}{Constants.SentenceEnd} {emotionDescription}";
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
