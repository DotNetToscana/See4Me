using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace See4Me.Services
{
    public class SettingsService : ISettingsService
    {
        private const string CAMERA_PANEL = "CameraPanel";
        private const string SHOW_DESCRIPTION_CONFIDENCE = "ShowDescriptionConfidence";
        private const string SHOW_EXCEPTION_ON_ERROR = "ShowExceptionOnError";
        private const string GUESS_AGE = "GuessAge";
        private const string SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION = "ShowOriginalDescriptionOnTranslation";
        private const string SHOW_RAW_DESCRIPTION_ON_INVALID_RECOGNITION = "ShowRawDescriptionOnInvalidRecognition";
        private const string VISION_SUBSCRIPTION_KEY = "VisionSubscriptionKey";
        private const string EMOTION_SUBSCRIPTION_KEY = "EmotionSubscriptionKey";
        private const string TRANSLATOR_CLIENT_ID = "TranslatorClientId";
        private const string TRANSLATOR_CLIENT_SECRET = "TranslatorClientSecret";
        private const string IS_TEXT_TO_SPEECH_ENABLED = "IsTextToSpeechEnabled";
        private const string IS_CONSENT_GIVEN = "IsConsentGiven";

        private readonly ISettings settings;

        public SettingsService()
        {
            settings = CrossSettings.Current;
        }

        public CameraPanel CameraPanel
        {
            get
            {
                var setting = settings.GetValueOrDefault(CAMERA_PANEL, CameraPanel.Back.ToString());
                return (CameraPanel)Enum.Parse(typeof(CameraPanel), setting);
            }
            set { settings.AddOrUpdateValue(CAMERA_PANEL, value.ToString()); }
        }

        public bool ShowDescriptionConfidence
        {
            get { return settings.GetValueOrDefault(SHOW_DESCRIPTION_CONFIDENCE, false); }
            set { settings.AddOrUpdateValue(SHOW_DESCRIPTION_CONFIDENCE, value); }
        }

        public bool GuessAge
        {
            get { return settings.GetValueOrDefault(GUESS_AGE, true); }
            set { settings.AddOrUpdateValue(GUESS_AGE, value); }
        }

        public bool ShowOriginalDescriptionOnTranslation
        {
            get { return settings.GetValueOrDefault(SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION, false); }
            set { settings.AddOrUpdateValue(SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION, value); }
        }

        public bool ShowExceptionOnError
        {
            get
            {
#if DEBUG
                return true;
#else
                return settings.GetValueOrDefault(SHOW_EXCEPTION_ON_ERROR, false);
#endif
            }
            set { settings.AddOrUpdateValue(SHOW_EXCEPTION_ON_ERROR, value); }
        }

        public bool ShowRawDescriptionOnInvalidRecognition
        {
            get { return settings.GetValueOrDefault(SHOW_RAW_DESCRIPTION_ON_INVALID_RECOGNITION, false); }
            set { settings.AddOrUpdateValue(SHOW_RAW_DESCRIPTION_ON_INVALID_RECOGNITION, value); }
        }

        public string VisionSubscriptionKey
        {
            get { return settings.GetValueOrDefault<string>(VISION_SUBSCRIPTION_KEY, null); }
            set { settings.AddOrUpdateValue(VISION_SUBSCRIPTION_KEY, value); }
        }

        public string EmotionSubscriptionKey
        {
            get { return settings.GetValueOrDefault<string>(EMOTION_SUBSCRIPTION_KEY, null); }
            set { settings.AddOrUpdateValue(EMOTION_SUBSCRIPTION_KEY, value); }
        }

        public string TranslatorClientId
        {
            get { return settings.GetValueOrDefault<string>(TRANSLATOR_CLIENT_ID, null); }
            set { settings.AddOrUpdateValue(TRANSLATOR_CLIENT_ID, value); }
        }

        public string TranslatorClientSecret
        {
            get { return settings.GetValueOrDefault<string>(TRANSLATOR_CLIENT_SECRET, null); }
            set { settings.AddOrUpdateValue(TRANSLATOR_CLIENT_SECRET, value); }
        }

        public bool IsTextToSpeechEnabled
        {
            get { return settings.GetValueOrDefault(IS_TEXT_TO_SPEECH_ENABLED, true); }
            set { settings.AddOrUpdateValue(IS_TEXT_TO_SPEECH_ENABLED, value); }
        }

        public bool IsConsentGiven
        {
            get
            {
#if DEBUG
                // When in debug mode, the consent is implicitly given.
                return true;
#else
                return settings.GetValueOrDefault(IS_CONSENT_GIVEN, false);
#endif
            }

            set { settings.AddOrUpdateValue(IS_CONSENT_GIVEN, value); }
        }
    }
}
