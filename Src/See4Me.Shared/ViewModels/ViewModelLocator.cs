using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using See4Me.Engine;
using See4Me.Engine.Services.ServiceSettings;
using See4Me.Services;
using See4Me.Services.ServiceSettings;
using System.Globalization;

namespace See4Me.ViewModels
{
    public partial class ViewModelLocator
    {
        public void Initialize(Services.INavigationService navigationService)
        {
            SimpleIoc.Default.Register<Services.INavigationService>(() => navigationService);
        }

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<CognitiveClient>(() =>
            {
                var visionSettingsProvider = new LocalVisionSettingsProvider();
                var cognitiveClient = new CognitiveClient(visionSettingsProvider);

                var cognitiveSettings = cognitiveClient.Settings;
                cognitiveSettings.EmotionSubscriptionKey = ServiceKeys.EmotionSubscriptionKey;
                cognitiveSettings.VisionSubscriptionKey = ServiceKeys.VisionSubscriptionKey;
                cognitiveSettings.TranslatorClientId = ServiceKeys.TranslatorClientId;
                cognitiveSettings.TranslatorClientSecret = ServiceKeys.TranslatorClientSecret;

                return cognitiveClient;
            });

            SimpleIoc.Default.Register<ISpeechService>(() =>
            {
                var language = GetLanguage();
                var service = new SpeechService { Language = language };
                return service;
            });

            SimpleIoc.Default.Register<Services.IDialogService, Services.DialogService>();
            SimpleIoc.Default.Register<IStreamingService, StreamingService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<INetworkService, NetworkService>();
            SimpleIoc.Default.Register<ILauncherService, LauncherService>();
            SimpleIoc.Default.Register<IAppService, AppService>();
            SimpleIoc.Default.Register<IMediaPicker, MediaPicker>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
            SimpleIoc.Default.Register<RecognizeTextViewModel>();
            SimpleIoc.Default.Register<PrivacyViewModel>();

            OnInitialize();
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();

        public SettingsViewModel SettingsViewModel => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public AboutViewModel AboutViewModel => ServiceLocator.Current.GetInstance<AboutViewModel>();

        public RecognizeTextViewModel RecognizeTextViewModel => ServiceLocator.Current.GetInstance<RecognizeTextViewModel>();

        public PrivacyViewModel PrivacyViewModel => ServiceLocator.Current.GetInstance<PrivacyViewModel>();

        static partial void OnInitialize();

        private static CultureInfo culture;
        public static string GetLanguage()
        {
            if (culture == null)
            {
#if __ANDROID__ || __IOS__
                var language = Localization.Resources.AppResources.ResourceLanguage;
#else
                var language = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
#endif

                culture = new CultureInfo(language);
                if (!culture.IsNeutralCulture)
                    culture = culture.Parent;
            }

            return culture.TwoLetterISOLanguageName;
        }
    }
}
