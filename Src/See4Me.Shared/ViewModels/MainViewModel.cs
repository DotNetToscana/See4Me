using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Vision;
using See4Me.Common;
using See4Me.Localization.Resources;
using See4Me.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Net;
using See4Me.Engine;
using Microsoft.Practices.ServiceLocation;
using See4Me.Engine.Services.ServiceSettings;
using See4Me.Extensions;

namespace See4Me.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IStreamingService streamingService;
        private readonly ISpeechService speechService;
        private readonly CognitiveClient cognitiveClient;

        private CameraPanel lastCameraPanel = CameraPanel.Unknown;
        private bool initialized = false;

        public bool IsVisionServiceRegistered => !string.IsNullOrWhiteSpace(ServiceKeys.VisionSubscriptionKey);

        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { this.Set(ref statusMessage, value, true); }
        }

        public AutoRelayCommand DescribeImageCommand { get; set; }

        public AutoRelayCommand SwapCameraCommand { get; set; }

        public AutoRelayCommand GotoSettingsCommand { get; set; }

        public AutoRelayCommand GotoRecognizeTextCommand { get; set; }

        public MainViewModel(CognitiveClient cognitiveClient, IStreamingService streamingService, ISpeechService speechService)
        {
            this.cognitiveClient = cognitiveClient;
            this.streamingService = streamingService;
            this.speechService = speechService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            DescribeImageCommand = new AutoRelayCommand(async () => await DescribeImageAsync(), () => IsVisionServiceRegistered && !IsBusy)
                .DependsOn(() => IsBusy);

            SwapCameraCommand = new AutoRelayCommand(async () => await SwapCameraAsync(), () => IsVisionServiceRegistered && !IsBusy)
                .DependsOn(() => IsBusy);

            GotoSettingsCommand = new AutoRelayCommand(() => Navigator.NavigateTo(Pages.SettingsPage.ToString()));

            GotoRecognizeTextCommand = new AutoRelayCommand(() => Navigator.NavigateTo(Pages.RecognizeTextPage.ToString()), () => IsVisionServiceRegistered && !IsBusy)
                .DependsOn(() => IsBusy);
        }

        public async Task CheckShowConsentAsync()
        {
            // If not given, asks the user for the consent to use the app.
            if (!Settings.IsConsentGiven)
            {
                await DialogService.ShowAsync(AppResources.ConsentRequiredMessage, AppResources.ConsentRequiredTitle);
                Settings.IsConsentGiven = true;
            }
        }

        public async Task InitializeStreamingAsync()
        {
            IsBusy = true;

            try
            {
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
                        if (!IsVisionServiceRegistered)
                        {
                            StatusMessage = AppResources.ServiceNotRegistered;
                            await SpeechHelper.TrySpeechAsync(AppResources.ServiceNotRegistered);
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

            initialized = true;
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

            MessengerInstance.Send(new NotificationMessage(Constants.TakingPhoto));

            try
            {
                StatusMessage = AppResources.QueryingVisionService;
                using (var stream = await streamingService.GetCurrentFrameAsync())
                {
                    if (stream != null)
                    {
                        var imageBytes = await stream.ToArrayAsync();
                        MessengerInstance.Send(new NotificationMessage<byte[]>(imageBytes, Constants.PhotoTaken));

                        if (await Network.IsInternetAvailableAsync())
                        {
                            var result = await cognitiveClient.RecognizeAsync(stream, Language, RecognitionType.Vision | RecognitionType.Emotion, OnRecognitionProgress);
                            var visionResult = result.VisionResult;

                            if (visionResult.IsValid)
                            {
                                baseDescription = visionResult.Description;
                                if (visionResult.IsTranslated)
                                {
                                    if (Settings.ShowOriginalDescriptionOnTranslation)
                                        baseDescription = $"{visionResult.TranslatedDescription} ({visionResult.Description})";
                                    else
                                        baseDescription = visionResult.TranslatedDescription;
                                }

                                if (Settings.ShowDescriptionConfidence)
                                    baseDescription = $"{baseDescription} ({Math.Round(visionResult.Confidence, 2)})";

                                // Analyzes emotion results.
                                var emotionResults = result.EmotionResults;

                                if (emotionResults.Any())
                                {
                                    var emotionMessages = new StringBuilder();

                                    foreach (var emotionResult in emotionResults)
                                    {
                                        var emotionMessage = SpeechHelper.GetEmotionMessage(emotionResult.Gender, emotionResult.Age, emotionResult.Emotion);
                                        if (!string.IsNullOrWhiteSpace(emotionMessage))
                                            emotionMessages.Append(emotionMessage);
                                    }

                                    // Describes how many faces have been recognized.
                                    if (emotionResults.Count() == 1)
                                        facesRecognizedDescription = AppResources.FaceRecognizedSingular;
                                    else
                                        facesRecognizedDescription = $"{string.Format(AppResources.FacesRecognizedPlural, emotionResults.Count())} {Constants.SentenceEnd}";

                                    emotionDescription = emotionMessages.ToString();
                                }
                            }
                            else
                            {
                                if (Settings.ShowRawDescriptionOnInvalidRecognition && visionResult.RawDescription != null)
                                    baseDescription = $"{AppResources.RecognitionFailed} ({visionResult.RawDescription}, {Math.Round(visionResult.Confidence, 2)})";
                                else
                                    baseDescription = AppResources.RecognitionFailed;
                            }
                        }
                        else
                        {
                            // Connection isn't available, the service cannot be reached.
                            baseDescription = AppResources.NoConnection;
                        }
                    }
                    else
                    {
                        baseDescription = AppResources.UnableToTakePhoto;
                    }
                }
            }
            catch (Microsoft.ProjectOxford.Vision.ClientException)
            {
                // Unable to access the service (tipically, due to invalid registration keys).
                baseDescription = AppResources.UnableToAccessService;
            }
            catch (Microsoft.ProjectOxford.Common.ClientException ex) when (ex.Error.Code.ToLower() == "unauthorized")
            {
                // Unable to access the service (tipically, due to invalid registration keys).
                baseDescription = AppResources.UnableToAccessService;
            }
            catch (WebException)
            {
                // Internet isn't available, the service cannot be reached.
                baseDescription = AppResources.NoConnection;
            }
            catch (Exception ex)
            {
                var error = AppResources.RecognitionError;

                if (Settings.ShowExceptionOnError)
                    error = $"{error} ({ex.Message})";

                baseDescription = error;
            }

            // Shows and speaks the result.
            var message = $"{baseDescription}{Constants.SentenceEnd} {facesRecognizedDescription} {emotionDescription}";
            StatusMessage = this.GetNormalizedMessage(message);

            await SpeechHelper.TrySpeechAsync(message);

            IsBusy = false;
        }

        private Task OnRecognitionProgress(RecognitionPhase phase)
        {
            switch (phase)
            {
                case RecognitionPhase.QueryingService:
                    StatusMessage = AppResources.QueryingVisionService;
                    break;

                case RecognitionPhase.Translating:
                    StatusMessage = AppResources.Translating;
                    break;

                case RecognitionPhase.RecognizingFaces:
                    StatusMessage = AppResources.RecognizingFaces;
                    break;
            }

            return Task.CompletedTask;
        }

        public async Task SwapCameraAsync()
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
                    await this.NotifySwapCameraErrorAsync();
            }

            IsBusy = false;
        }

        private async Task NotifyCameraPanelAsync()
        {
            // Avoids to notify if the camera panel is the same.
            if (streamingService.CameraPanel != lastCameraPanel)
            {
                var message = streamingService.CameraPanel == CameraPanel.Front ? AppResources.FrontCameraReady : AppResources.BackCameraReady;
                StatusMessage = message;

                await SpeechHelper.TrySpeechAsync(message);
                lastCameraPanel = streamingService.CameraPanel;
            }
        }

        private async Task NotifyInitializationErrorAsync(Exception error = null)
        {
            // If the app is already initialized, skips the notification error.
            if (!initialized)
            {
                var errorMessage = AppResources.InitializationError;
                if (error != null && Settings.ShowExceptionOnError)
                    errorMessage = $"{errorMessage} ({error.Message})";

                StatusMessage = errorMessage;

                await SpeechHelper.TrySpeechAsync(errorMessage);
            }
        }

        private async Task NotifySwapCameraErrorAsync()
        {
            StatusMessage = AppResources.SwapCameraError;
            await SpeechHelper.TrySpeechAsync(StatusMessage);
        }

        private string GetNormalizedMessage(string message)
            => message.Replace(Constants.SentenceEnd, ". ").TrimEnd('.').Trim().Replace("  ", " ").Replace(" .", ".").Replace("..", ".");
    }
}