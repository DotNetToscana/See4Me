namespace See4Me.Services
{
    public interface ISettingsService
    {
        CameraPanel CameraPanel { get; set; }

        bool ShowDescriptionConfidence { get; set; }

        bool ShowExceptionOnError { get; set; }

        bool GuessAge { get; set; }

        bool ShowOriginalDescriptionOnTranslation { get; set; }

        bool ShowRawDescriptionOnInvalidRecognition { get; set; }

        string VisionSubscriptionKey { get; set; }

        string EmotionSubscriptionKey { get; set; }

        string TranslatorClientId { get; set; }

        string TranslatorClientSecret { get; set; }

        bool IsTextToSpeechEnabled { get; set; }

        bool IsConsentGiven { get; set; }
    }
}
