namespace See4Me.Services
{
    public interface ISettingsService
    {
        CameraPanel CameraPanel { get; set; }

        bool AutomaticTranslation { get; set; }
    }
}
