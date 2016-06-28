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
    }
}
