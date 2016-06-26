using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Emotion;
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
using See4Me.Extensions;
using System.IO;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.ProjectOxford.Vision.Contract;

namespace See4Me.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IStreamingService streamingService;
        private readonly EmotionServiceClient emotionService;
        private readonly VisionServiceClient visionService;
        private readonly ITranslatorService translatorService;
        private readonly ISpeechService speechService;

        public bool IsServiceRegistered
            => !string.IsNullOrWhiteSpace(ServiceKeys.VisionSubscriptionKey) && !string.IsNullOrWhiteSpace(ServiceKeys.EmotionSubscriptionKey)
            && !string.IsNullOrWhiteSpace(ServiceKeys.TranslatorClientId) && !string.IsNullOrWhiteSpace(ServiceKeys.TranslatorClientSecret);

        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { this.Set(ref statusMessage, value); }
        }

        public AutoRelayCommand VideoCommand { get; set; }

        public AutoRelayCommand<SwipeDirection> SwipeCommand { get; set; }

        public AutoRelayCommand GuessAgeCommand { get; set; }

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
            VideoCommand = new AutoRelayCommand(async () => await DescribeImageAsync(), () => IsServiceRegistered && !IsBusy)
                .DependsOn(() => IsBusy);

            SwipeCommand = new AutoRelayCommand<SwipeDirection>(async (direction) => await SwapCameraAsync(direction), (direction) => !IsBusy).
                DependsOn(() => IsBusy);

            GuessAgeCommand = new AutoRelayCommand(async () => await SetGuessAgeAsync());
        }

        public async Task InitializeAsync()
        {
            IsBusy = true;

            try
            {
                if (IsConnected && Language != Constants.DefaultLanguge)
                {
                    // Retrieves tha authorization token for the translator service.
                    // This is necessary only if the app language is different from the default language,
                    // otherwise no translation will be performed.
                    var task = translatorService.InitializeAsync();
                }

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
                        if (!IsServiceRegistered)
                        {
                            StatusMessage = AppResources.ServiceNotRegistered;
                            await speechService.SpeechAsync(AppResources.ServiceNotRegistered);
                        }
                        else if (successful)
                        {
                            await this.NotifyCameraPanelAsync();
                        }
                        else
                        {
                            await this.NotifyInitializationErrorAsync();
                        }
                    }
                }));
            }
            catch
            {
                await this.NotifyInitializationErrorAsync();
            }

            IsBusy = false;
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
            StatusMessage = null;

            string baseDescription = null;
            string facesRecognizedDescription = null;
            string emotionDescription = null;

            MessengerInstance.Send(new NotificationMessage(Constants.TakePhoto));

            if (IsConnected && await Network.GetIsInternetAvailableAsync())
            {
                try
                {
                    using (var stream = await streamingService.GetCurrentFrameAsync())
                    {
                        if (stream != null)
                        {
                            StatusMessage = AppResources.QueryingVisionService;

                            var visualFeatures = new VisualFeature[] { VisualFeature.Description, VisualFeature.Faces };
                            var result = await visionService.AnalyzeImageAsync(stream, visualFeatures);

                            Caption description;
                            if (result.IsValid(out description))
                            {
                                baseDescription = description.Text;

                                if (Settings.ShowDescriptionConfidence)
                                    baseDescription = $"{baseDescription} ({Math.Round(description.Confidence, 2)})";

                                if (Language != Constants.DefaultLanguge)
                                {
                                    // The description needs to be translated.
                                    if (!translatorService.IsInitialized && IsConnected)
                                        await this.translatorService.InitializeAsync();

                                    if (translatorService.IsInitialized)
                                    {
                                        StatusMessage = AppResources.Translating;
                                        var translation = await translatorService.TranslateAsync(baseDescription);

                                        if (Settings.ShowOriginalDescriptionOnTranslation)
                                            baseDescription = $"{translation} ({baseDescription})";
                                        else
                                            baseDescription = translation;
                                    }
                                }

                                StatusMessage = baseDescription;

                                try
                                {
                                    // If there is one or more faces, asks the service information about them.
                                    if (result.Faces?.Count() > 0)
                                    {
                                        StatusMessage = AppResources.RecognizingFaces;

                                        var messages = new StringBuilder();
                                        var imageBytes = await stream.ToArrayAsync();

                                        foreach (var face in result.Faces)
                                        {
                                            using (var ms = new MemoryStream(imageBytes))
                                            {
                                                var emotions = await emotionService.RecognizeAsync(ms, face.FaceRectangle.ToRectangle());
                                                var bestEmotion = emotions.FirstOrDefault()?.Scores.GetBestEmotion();

                                                // Creates the emotion description text to be speeched (if there are interesting information).
                                                var emotionMessage = SpeechHelper.GetEmotionMessage(face, bestEmotion, includeAge: Settings.GuessAge);
                                                if (!string.IsNullOrWhiteSpace(emotionMessage))
                                                    messages.Append(emotionMessage);
                                            }
                                        }

                                        // Checks if at least an emotion has been actually recognized.
                                        if (messages.Length > 0)
                                        {
                                            // Describes how many faces have been recognized.
                                            if (result.Faces.Count() == 1)
                                                facesRecognizedDescription = AppResources.FaceRecognizedSingular;
                                            else
                                                facesRecognizedDescription = $"{string.Format(AppResources.FacesRecognizedPlural, result.Faces.Count())} {Constants.SentenceEnd}";

                                            emotionDescription = messages.ToString();
                                        }
                                    }
                                }
                                catch { }
                            }
                            else
                            {
                                baseDescription = AppResources.RecognitionFailed;
                            }
                        }
                        else
                        {
                            baseDescription = AppResources.UnableToGetImage;
                        }
                    }
                }
                catch (WebException)
                {
                    // Internet isn't available, to service cannot be reached.
                    baseDescription = AppResources.NoConnection;
                }
                catch (Exception ex)
                {
                    var error = AppResources.RecognitionError;

                    if (Settings.ShowExceptionOnError)
                        error = $"{error} ({ex.Message})";

                    baseDescription = error;
                }
            }
            else
            {
                // Internet isn't available, to service cannot be reached.
                baseDescription = AppResources.NoConnection;
            }

            // Speaks the result.
            var message = $"{baseDescription}{Constants.SentenceEnd} {facesRecognizedDescription} {emotionDescription}";
            StatusMessage = this.GetNormalizedMessage(message);
            message = this.GetSpeechMessage(message);
            await speechService.SpeechAsync(message, languge: Language);

            IsBusy = false;
        }

        public async Task SwapCameraAsync(SwipeDirection direction)
        {
            IsBusy = true;
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

            IsBusy = false;
        }

        private async Task SetGuessAgeAsync()
        {
            Settings.GuessAge = !Settings.GuessAge;

            var message = Settings.GuessAge ? AppResources.GuessAge : AppResources.DontGuessAge;
            StatusMessage = message;

            await speechService.SpeechAsync(message);
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

        private string GetNormalizedMessage(string message)
            => message.Replace(Constants.SentenceEnd, ". ").TrimEnd('.').Trim().Replace("  ", " ").Replace(" .", ".").Replace("..", ".");

        private string GetSpeechMessage(string message)
            => Regex.Replace(message, @" ?\(.*?\)", string.Empty);
    }
}
