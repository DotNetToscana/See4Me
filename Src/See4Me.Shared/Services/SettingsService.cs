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
            get { return settings.GetValueOrDefault(SHOW_DESCRIPTION_CONFIDENCE, true); }
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

        public bool ShowOriginalDescriptionOnTranslation
        {
            get { return settings.GetValueOrDefault(SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION, true); }
            set { settings.AddOrUpdateValue(SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION, value); }
        }

        public bool ShowRawDescriptionOnInvalidRecognition
        {
            get { return settings.GetValueOrDefault(SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION, true); }
            set { settings.AddOrUpdateValue(SHOW_ORIGINAL_DESCRIPTION_ON_TRANSLATION, value); }
        }
    }
}
