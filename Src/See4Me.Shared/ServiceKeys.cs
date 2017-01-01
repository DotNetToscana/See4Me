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
        private const string faceSubscriptionKey = "";
        private const string translatorSubscriptionKey = "";
#else
        private const string visionSubscriptionKey = "";
        private const string emotionSubscriptionKey = "";
        private const string faceSubscriptionKey = "";
        private const string translatorSubscriptionKey = "";
#endif

        private static readonly ISettingsService settings;

        static ServiceKeys()
        {
            settings = ServiceLocator.Current.GetInstance<ISettingsService>();
        }

        public static string VisionSubscriptionKey
        {
            get { return GetValue(settings.VisionSubscriptionKey) ?? visionSubscriptionKey; }
            set { settings.VisionSubscriptionKey = value; }
        }

        public static string EmotionSubscriptionKey
        {
            get { return GetValue(settings.EmotionSubscriptionKey) ?? emotionSubscriptionKey; }
            set { settings.EmotionSubscriptionKey = value; }
        }

        public static string FaceSubscriptionKey
        {
            get { return GetValue(settings.FaceSubscriptionKey) ?? faceSubscriptionKey; }
            set { settings.FaceSubscriptionKey = value; }
        }

        public static string TranslatorSubscriptionKey
        {
            get { return GetValue(settings.TranslatorSubscriptionKey) ?? translatorSubscriptionKey; }
            set { settings.TranslatorSubscriptionKey = value; }
        }

        private static string GetValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return null;
        }
    }
}
