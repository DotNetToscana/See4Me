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
using See4Me.Extensions;
using System.IO;
using System.Text;
using System.Net;
using See4Me.Engine;

namespace See4Me.ViewModels
{
    public partial class RecognizeTextViewModel : ViewModelBase
    {
        private readonly IMediaPicker mediaPicker;
        private readonly ISpeechService speechService;
        private readonly CognitiveClient cognitiveClient;

        public AutoRelayCommand TakePhotoCommand { get; set; }

        public string message;
        public string Message
        {
            get { return message; }
            set { this.Set(ref message, value, broadcast: true); }
        }

        public RecognizeTextViewModel(CognitiveClient cognitiveClient, IMediaPicker mediaPicker, ISpeechService speechService)
        {
            this.cognitiveClient = cognitiveClient;
            this.mediaPicker = mediaPicker;
            this.speechService = speechService;

            this.CreateCommands();
        }

        private void CreateCommands()
        {
            TakePhotoCommand = new AutoRelayCommand(async () => await TakePhotoAsync(), () => !IsBusy).DependsOn(() => IsBusy);
        }

        private async Task TakePhotoAsync()
        {
            IsBusy = true;

            string recognizeText = null;

            try
            {
                using (var stream = await mediaPicker.TakePhotoAsync())
                {
                    if (stream != null)
                    {
                        var imageBytes = await stream.ToArrayAsync();
                        MessengerInstance.Send(new NotificationMessage<byte[]>(imageBytes, Constants.PhotoTaken));
                        Message = null;

                        if (await NetworkService.IsInternetAvailableAsync())
                        {
                            var result = await cognitiveClient.RecognizeAsync(stream, Language, RecognitionType.Text);
                            var ocrResult = result.OcrResult;

                            if (ocrResult.IsValid)
                                recognizeText = ocrResult.Text;
                            else
                                recognizeText = AppResources.UnableToRecognizeText;
                        }
                        else
                        {
                            // Internet isn't available, the service cannot be reached.
                            recognizeText = AppResources.NoConnection;
                        }
                    }
                    else
                    {
                        // If message is null at this point, this is the first request. If we cancel it, turns automatically to the
                        // previous page.
                        if (message == null)
                            AppNavigationService.GoBack();

                        IsBusy = false;
                        return;
                    }
                }
            }
            catch (WebException)
            {
                // Internet isn't available, the service cannot be reached.
                recognizeText = AppResources.NoConnection;
            }
            catch (ClientException)
            {
                // Unable to access the service (tipically, due to invalid registration keys).
                recognizeText = AppResources.UnableToAccessService;
            }
            catch (Exception ex)
            {
                var error = AppResources.RecognitionError;

                if (Settings.ShowExceptionOnError)
                    error = $"{error} ({ex.Message})";

                recognizeText = error;
            }

            // Shows the result.
            Message = this.GetNormalizedMessage(recognizeText);
            IsBusy = false;
        }

        private string GetNormalizedMessage(string message) => message;
    }
}
