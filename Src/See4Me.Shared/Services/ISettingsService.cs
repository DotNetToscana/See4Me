namespace See4Me.Services
{
    public interface ISettingsService
    {
        CameraPanel CameraPanel { get; set; }

        bool ShowDescriptionConfidence { get; set; }

        bool ShowExceptionOnError { get; set; }

        bool ShowOriginalDescriptionOnTranslation { get; set; }

        bool ShowRawDescriptionOnInvalidRecognition { get; set; }

        string VisionSubscriptionKey { get; set; }

        string EmotionSubscriptionKey { get; set; }

        string TranslatorSubscriptionKey { get; set; }

        bool IsTextToSpeechEnabled { get; set; }

        bool IsConsentGiven { get; set; }
    }
}
