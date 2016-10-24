using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using See4Me.Common;
using See4Me.Localization.Resources;
using See4Me.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using See4Me.Extensions;
using System.IO;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;
using See4Me.Engine;

namespace See4Me.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly ILauncherService launcherService;
        private readonly CognitiveClient cognitiveClient;

        public AutoRelayCommand SubscribeCognitiveServicesCommand { get; set; }

        public AutoRelayCommand ActivateTranslatorServiceCommand { get; set; }

        public AutoRelayCommand CreateTranslatorAppCommand { get; set; }

        public AutoRelayCommand GotoAboutCommand { get; set; }

        public AutoRelayCommand GotoPrivacyPolicyCommand { get; set; }

        public AutoRelayCommand SaveCommand { get; set; }

        private string visionSubscriptionKey;
        public string VisionSubscriptionKey
        {
            get { return visionSubscriptionKey; }
            set { this.Set(ref visionSubscriptionKey, value); }
        }

        private string emotionSubscriptionKey;
        public string EmotionSubscriptionKey
        {
            get { return emotionSubscriptionKey; }
            set { this.Set(ref emotionSubscriptionKey, value); }
        }

        private string translatorClientId;
        public string TranslatorClientId
        {
            get { return translatorClientId; }
            set { this.Set(ref translatorClientId, value); }
        }

        private string translatorClientSecret;
        public string TranslatorClientSecret
        {
            get { return translatorClientSecret; }
            set { this.Set(ref translatorClientSecret, value); }
        }

        private bool isTextToSpeechEnabled;
        public bool IsTextToSpeechEnabled
        {
            get { return isTextToSpeechEnabled; }
            set { this.Set(ref isTextToSpeechEnabled, value); }
        }

        private bool showDescriptionConfidence;
        public bool ShowDescriptionConfidence
        {
            get { return showDescriptionConfidence; }
            set { this.Set(ref showDescriptionConfidence, value); }
        }

        private bool showOriginalDescriptionOnTranslation;
        public bool ShowOriginalDescriptionOnTranslation
        {
            get { return showOriginalDescriptionOnTranslation; }
            set { this.Set(ref showOriginalDescriptionOnTranslation, value); }
        }

        public SettingsViewModel(CognitiveClient cognitiveClient, ILauncherService launcherService)
        {
            this.cognitiveClient = cognitiveClient;
            this.launcherService = launcherService;

            this.CreateCommands();
        }

        public void Initialize()
        {
            VisionSubscriptionKey = ServiceKeys.VisionSubscriptionKey;
            EmotionSubscriptionKey = ServiceKeys.EmotionSubscriptionKey;
            TranslatorClientId = ServiceKeys.TranslatorClientId;
            TranslatorClientSecret = ServiceKeys.TranslatorClientSecret;

            IsTextToSpeechEnabled = Settings.IsTextToSpeechEnabled;
            ShowDescriptionConfidence = Settings.ShowDescriptionConfidence;
            ShowOriginalDescriptionOnTranslation = Settings.ShowOriginalDescriptionOnTranslation;
        }

        private void CreateCommands()
        {
            SaveCommand = new AutoRelayCommand(Save);

            SubscribeCognitiveServicesCommand = new AutoRelayCommand(() => launcherService.LaunchUriAsync(Constants.CognitiveServicesSubscriptionUrl));
            ActivateTranslatorServiceCommand = new AutoRelayCommand(() => launcherService.LaunchUriAsync(Constants.ActivateTranslatorServiceUrl));
            CreateTranslatorAppCommand = new AutoRelayCommand(() => launcherService.LaunchUriAsync(Constants.TranslatorServiceCreateAppUrl));
            GotoAboutCommand = new AutoRelayCommand(() => Navigator.NavigateTo(Pages.AboutPage.ToString()));
            GotoPrivacyPolicyCommand = new AutoRelayCommand(() => Navigator.NavigateTo(Pages.PrivacyPolicyPage.ToString()));
        }

        public void Save()
        {
            ServiceKeys.VisionSubscriptionKey = visionSubscriptionKey;
            ServiceKeys.EmotionSubscriptionKey = emotionSubscriptionKey;
            ServiceKeys.TranslatorClientId = translatorClientId;
            ServiceKeys.TranslatorClientSecret = TranslatorClientSecret;

            Settings.IsTextToSpeechEnabled = isTextToSpeechEnabled;
            Settings.ShowDescriptionConfidence = showDescriptionConfidence;
            Settings.ShowOriginalDescriptionOnTranslation = showOriginalDescriptionOnTranslation;

            var cognitiveSettings = cognitiveClient.Settings;
            cognitiveSettings.EmotionSubscriptionKey = emotionSubscriptionKey;
            cognitiveSettings.VisionSubscriptionKey = visionSubscriptionKey;
            cognitiveSettings.TranslatorClientId = translatorClientId;
            cognitiveSettings.TranslatorClientSecret = translatorClientSecret;

            Navigator.GoBack();
        }
    }
}
