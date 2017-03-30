using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using See4Me.Common;
using See4Me.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Template10.Services.NavigationService;
using Microsoft.Practices.ServiceLocation;
using System;

namespace See4Me.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private const string SHOW_RECOGNITION_CONFIDENCE = "ShowRecognitionConfidence";
        private const string SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION = "ShowOriginalDescriptionOnTranslation";
        private const string VISION_SUBSCRIPTION_KEY = "VisionSubscriptionKey";
        private const string FACE_SUBSCRIPTION_KEY = "FaceSubscriptionKey";
        private const string EMOTION_SUBSCRIPTION_KEY = "EmotionSubscriptionKey";
        private const string TRANSLATOR_SUBSCRIPTION_KEY = "TranslatorSubscriptionKey";
        private const string IS_TEXT_TO_SPEECH_ENABLED = "IsTextToSpeechEnabled";
        private const string SHOW_DESCRIPTION_ON_FACE_IDENTIFICATION = "ShowDescriptionOnFaceIdentification";

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (mode != NavigationMode.Back)
            {
                this.Initialize();
            }

            if (state.Any())
            {
                try
                {
                    this.Restore(state);
                }
                catch { }
                finally
                {
                    state.Clear();
                }
            }

            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            this.Save();      
            return base.OnNavigatingFromAsync(args);
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {
                state[SHOW_RECOGNITION_CONFIDENCE] = showRecognitionConfidence;
                state[SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION] = showOriginalDescriptionOnTranslation;
                state[VISION_SUBSCRIPTION_KEY] = visionSubscriptionKey;
                state[TRANSLATOR_SUBSCRIPTION_KEY] = translatorSubscriptionKey;
                state[IS_TEXT_TO_SPEECH_ENABLED] = isTextToSpeechEnabled;
                state[SHOW_DESCRIPTION_ON_FACE_IDENTIFICATION] = showDescriptionOnFaceIdentification;
            }

            return base.OnNavigatedFromAsync(state, suspending);
        }

        private void Restore(IDictionary<string, object> state)
        {
            showRecognitionConfidence = Convert.ToBoolean(state[SHOW_RECOGNITION_CONFIDENCE]);
            showOriginalDescriptionOnTranslation = Convert.ToBoolean(state[SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION]);
            visionSubscriptionKey = state[VISION_SUBSCRIPTION_KEY].ToString();
            translatorSubscriptionKey = state[TRANSLATOR_SUBSCRIPTION_KEY].ToString();
            faceSubscriptionKey = state[FACE_SUBSCRIPTION_KEY].ToString();
            isTextToSpeechEnabled = Convert.ToBoolean(state[IS_TEXT_TO_SPEECH_ENABLED]);
            showDescriptionOnFaceIdentification = Convert.ToBoolean(state[SHOW_DESCRIPTION_ON_FACE_IDENTIFICATION]);
        }
    }
}
