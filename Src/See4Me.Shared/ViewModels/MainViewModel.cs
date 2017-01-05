using GalaSoft.MvvmLight.Messaging;
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
using See4Me.Extensions;

namespace See4Me.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IStreamingService streamingService;
        private readonly ISpeechService speechService;
        private readonly CognitiveClient cognitiveClient;
        private readonly ILauncherService launcherService;

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

        public AutoRelayCommand HowToRegisterCommand { get; set; }

        public MainViewModel(CognitiveClient cognitiveClient, IStreamingService streamingService, ISpeechService speechService, ILauncherService launcherService)
        {
            this.cognitiveClient = cognitiveClient;
            this.streamingService = streamingService;
            this.speechService = speechService;
            this.launcherService = launcherService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            DescribeImageCommand = new AutoRelayCommand(async () => await DescribeImageAsync(), () => IsVisionServiceRegistered && !IsBusy)
                .DependsOn(() => IsBusy);

            SwapCameraCommand = new AutoRelayCommand(async () => await SwapCameraAsync(), () => !IsBusy)
                .DependsOn(() => IsBusy);

            GotoSettingsCommand = new AutoRelayCommand(() => AppNavigationService.NavigateTo(Pages.SettingsPage.ToString()));

            GotoRecognizeTextCommand = new AutoRelayCommand(() => AppNavigationService.NavigateTo(Pages.RecognizeTextPage.ToString()), () => IsVisionServiceRegistered && !IsBusy)
                .DependsOn(() => IsBusy);

            HowToRegisterCommand = new AutoRelayCommand(() => launcherService.LaunchUriAsync(Constants.HowToRegisterUrl),
              () => !IsVisionServiceRegistered).DependsOn(() => IsVisionServiceRegistered);

            OnCreateCommands();
        }

        partial void OnCreateCommands();

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

            string visionDescription = null;
            string facesRecognizedDescription = null;
            string facesDescription = null;

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

                        if (await NetworkService.IsInternetAvailableAsync())
                        {
                            var result = await cognitiveClient.AnalyzeAsync(stream, Language, RecognitionType.Vision | RecognitionType.Face | RecognitionType.Emotion, OnRecognitionProgress);

                            var visionResult = result.VisionResult;
                            var faceResults = result.FaceResults;

                            if (!faceResults.Any() || Settings.ShowDescriptionOnFaceIdentification)
                            {
                                // Gets the description only if no faces has been recognized or if the corresponding setting flag is set.
                                if (visionResult.IsValid)
                                {
                                    visionDescription = visionResult.Description;
                                    if (visionResult.IsTranslated)
                                    {
                                        if (Settings.ShowOriginalDescriptionOnTranslation)
                                            visionDescription = $"{visionResult.TranslatedDescription} ({visionResult.Description})";
                                        else
                                            visionDescription = visionResult.TranslatedDescription;
                                    }

                                    if (Settings.ShowDescriptionConfidence)
                                        visionDescription = $"{visionDescription} ({Math.Round(visionResult.Confidence, 2)})";
                                }
                                else
                                {
                                    if (Settings.ShowRawDescriptionOnInvalidRecognition && visionResult.RawDescription != null)
                                        visionDescription = $"{AppResources.RecognitionFailed} ({visionResult.RawDescription}, {Math.Round(visionResult.Confidence, 2)})";
                                    else
                                        visionDescription = AppResources.RecognitionFailed;
                                }

                                visionDescription = $"{visionDescription}{Constants.SentenceEnd}";
                            }
                            
                            if (faceResults.Any())
                            {
                                // At least a face has been recognized.
                                var faceMessages = new StringBuilder();

                                foreach (var faceResult in faceResults)
                                {
                                    var faceMessage = SpeechHelper.GetFaceMessage(faceResult);
                                    faceMessages.Append(faceMessage);
                                }

                                // Describes how many faces have been recognized.
                                if (faceResults.Count() == 1)
                                    facesRecognizedDescription = AppResources.FaceRecognizedSingular;
                                else
                                    facesRecognizedDescription = $"{string.Format(AppResources.FacesRecognizedPlural, faceResults.Count())} {Constants.SentenceEnd}";

                                facesDescription = faceMessages.ToString();
                            }
                        }
                        else
                        {
                            // Connection isn't available, the service cannot be reached.
                            visionDescription = AppResources.NoConnection;
                        }
                    }
                    else
                    {
                        visionDescription = AppResources.UnableToTakePhoto;
                    }
                }
            }
            catch (CognitiveException ex)
            {
                // Unable to access the service (message contains translated error details).
                visionDescription = ex.Message;
            }
            catch (WebException)
            {
                // Internet isn't available, the service cannot be reached.
                visionDescription = AppResources.NoConnection;
            }
            catch (Exception ex)
            {
                var error = AppResources.RecognitionError;

                if (Settings.ShowExceptionOnError)
                    error = $"{error} ({ex.Message})";

                visionDescription = error;
            }

            // Shows and speaks the result.
            var message = $"{visionDescription} {facesRecognizedDescription} {facesDescription}";
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