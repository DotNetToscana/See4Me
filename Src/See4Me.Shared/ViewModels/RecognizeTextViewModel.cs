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
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;

namespace See4Me.ViewModels
{
    public partial class RecognizeTextViewModel : ViewModelBase
    {
        private readonly IMediaPicker mediaPicker;
        private readonly ISpeechService speechService;

        public AutoRelayCommand TakePhotoCommand { get; set; }

        public string message;
        public string Message
        {
            get { return message; }
            set { this.Set(ref message, value, broadcast: true); }
        }

        public bool IsTranslatorServiceRegistered
            => !string.IsNullOrWhiteSpace(ServiceKeys.TranslatorClientId) && !string.IsNullOrWhiteSpace(ServiceKeys.TranslatorClientSecret);

        public RecognizeTextViewModel(IMediaPicker mediaPicker, ISpeechService speechService)
        {
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

            var visionService = ViewModelLocator.VisionServiceClient;
            var translatorService = ViewModelLocator.TranslatorService;

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

                        if (await Network.IsInternetAvailableAsync())
                        {
                            var results = await visionService.RecognizeTextAsync(stream);
                            var text = results.GetRecognizedText();

                            if (string.IsNullOrWhiteSpace(text))
                                recognizeText = AppResources.UnableToRecognizeText;
                            else
                                recognizeText = text;
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
                            Navigator.GoBack();

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
