using Microsoft.Practices.ServiceLocation;
using See4Me.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace See4Me
{
    public static class ServiceKeys
    {
#if DEBUG
        private const string visionSubscriptionKey = "";
        private const string emotionSubscriptionKey = "";
        private const string translatorClientId = "";
        private const string translatorClientSecret = "";
#else
        private const string visionSubscriptionKey = "";
        private const string emotionSubscriptionKey = "";
        private const string translatorClientId = "";
        private const string translatorClientSecret = "";

#endif

        private static readonly ISettingsService settings;

        static ServiceKeys()
        {
            settings = ServiceLocator.Current.GetInstance<ISettingsService>();
        }

        public static string VisionSubscriptionKey
        {
            get { return settings.VisionSubscriptionKey ?? visionSubscriptionKey; }
            set { settings.VisionSubscriptionKey = value; }
        }

        public static string EmotionSubscriptionKey
        {
            get { return settings.EmotionSubscriptionKey ?? emotionSubscriptionKey; }
            set { settings.EmotionSubscriptionKey = value; }
        }

        public static string TranslatorClientId
        {
            get { return settings.TranslatorClientId ?? translatorClientId; }
            set { settings.TranslatorClientId = value; }
        }

        public static string TranslatorClientSecret
        {
            get { return settings.TranslatorClientSecret ?? translatorClientSecret; }
            set { settings.TranslatorClientSecret = value; }
        }
    }
}
