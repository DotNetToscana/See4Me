using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Vision;
using See4Me.Common;
using See4Me.Services;
using See4Me.Services.Translator;
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
        private const string SHOW_DESCRIPTION_CONFIDENCE = "ShowDescriptionConfidence";
        private const string GUESS_AGE = "GuessAge";
        private const string SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION = "ShowOriginalDescriptionOnTranslation";
        private const string VISION_SUBSCRIPTION_KEY = "VisionSubscriptionKey";
        private const string EMOTION_SUBSCRIPTION_KEY = "EmotionSubscriptionKey";
        private const string TRANSLATOR_CLIENT_ID = "TranslatorClientId";
        private const string TRANSLATOR_CLIENT_SECRET = "TranslatorClientSecret";
        private const string IS_TEXT_TO_SPEECH_ENABLED = "IsTextToSpeechEnabled";

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (mode != NavigationMode.Back)
            {
                this.Initialize();
            }
            else if (state.Any())
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

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {
                state[SHOW_DESCRIPTION_CONFIDENCE] = showDescriptionConfidence;
                state[GUESS_AGE] = guessAge;
                state[SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION] = showOriginalDescriptionOnTranslation;
                state[VISION_SUBSCRIPTION_KEY] = visionSubscriptionKey;
                state[EMOTION_SUBSCRIPTION_KEY] = emotionSubscriptionKey;
                state[TRANSLATOR_CLIENT_ID] = translatorClientId;
                state[TRANSLATOR_CLIENT_SECRET] = translatorClientSecret;
                state[IS_TEXT_TO_SPEECH_ENABLED] = isTextToSpeechEnabled;
            }

            return base.OnNavigatedFromAsync(state, suspending);
        }

        private void Restore(IDictionary<string, object> state)
        {
            showDescriptionConfidence = Convert.ToBoolean(state[SHOW_DESCRIPTION_CONFIDENCE]);
            guessAge = Convert.ToBoolean(state[GUESS_AGE]);
            showOriginalDescriptionOnTranslation = Convert.ToBoolean(state[SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION]);
            visionSubscriptionKey = state[VISION_SUBSCRIPTION_KEY].ToString();
            emotionSubscriptionKey = state[EMOTION_SUBSCRIPTION_KEY].ToString();
            translatorClientId = state[TRANSLATOR_CLIENT_ID].ToString();
            translatorClientSecret = state[TRANSLATOR_CLIENT_SECRET].ToString();
            isTextToSpeechEnabled = Convert.ToBoolean(state[IS_TEXT_TO_SPEECH_ENABLED]);
        }
    }
}
