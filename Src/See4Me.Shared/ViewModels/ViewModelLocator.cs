using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using See4Me.Engine;
using See4Me.Engine.Services.ServiceSettings;
using See4Me.Services;
using System.Globalization;
using System.IO;
using System.Reflection;

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
                VisionSettings visionSettings = null;
                using (var stream = typeof(ViewModelLocator).GetTypeInfo().Assembly.GetManifestResourceStream(Constants.VisionSettingsFile))
                {
                    using (var reader = new StreamReader(stream))
                        visionSettings = JsonConvert.DeserializeObject<VisionSettings>(reader.ReadToEnd());
                }

                var visionSettingsProvider = new SimpleVisionSettingsProvider(visionSettings);
                var cognitiveClient = new CognitiveClient(visionSettingsProvider);

                var cognitiveSettings = cognitiveClient.Settings;
                cognitiveSettings.VisionSubscriptionKey = ServiceKeys.VisionSubscriptionKey;
                cognitiveSettings.FaceSubscriptionKey = ServiceKeys.FaceSubscriptionKey;
                cognitiveSettings.TranslatorSubscriptionKey = ServiceKeys.TranslatorSubscriptionKey;

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
