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

        public bool ShowExceptionOnError
        {
            get { return settings.GetValueOrDefault(SHOW_EXCEPTION_ON_ERROR, false); }
            set { settings.AddOrUpdateValue(SHOW_EXCEPTION_ON_ERROR, value); }
        }

        public bool GuessAge
        {
            get { return settings.GetValueOrDefault(GUESS_AGE, false); }
            set { settings.AddOrUpdateValue(GUESS_AGE, value); }
        }
    }
}
