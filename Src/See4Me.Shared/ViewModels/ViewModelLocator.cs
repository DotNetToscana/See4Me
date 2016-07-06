using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using See4Me.Services;
using See4Me.Services.Translator;
using System.Globalization;

namespace See4Me.ViewModels
{
    public partial class ViewModelLocator
    {
        public void Initialize(INavigationService navigationService = null)
        {
            if (navigationService != null)
                SimpleIoc.Default.Register<INavigationService>(() => navigationService);
        }

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            InitializeServices();

            SimpleIoc.Default.Register<ISpeechService>(() =>
            {
                var language = GetLanguage();
                var service = new SpeechService { Language = language };
                return service;
            });

            SimpleIoc.Default.Register<IStreamingService, StreamingService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<INetworkService, NetworkService>();
            SimpleIoc.Default.Register<ILauncherService, LauncherService>();
            SimpleIoc.Default.Register<IAppService, AppService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();

            OnInitialize();
        }

        public static void InitializeServices()
        {
            var language = GetLanguage();

            if (SimpleIoc.Default.IsRegistered<ITranslatorService>())
                SimpleIoc.Default.Unregister<ITranslatorService>();

            if (SimpleIoc.Default.IsRegistered<VisionServiceClient>())
                SimpleIoc.Default.Unregister<VisionServiceClient>();

            if (SimpleIoc.Default.IsRegistered<EmotionServiceClient>())
                SimpleIoc.Default.Unregister<EmotionServiceClient>();

            SimpleIoc.Default.Register<ITranslatorService>(() =>
            {
                var service = new TranslatorService(ServiceKeys.TranslatorClientId, ServiceKeys.TranslatorClientSecret, language);
                return service;
            });

            SimpleIoc.Default.Register<VisionServiceClient>(() => new VisionServiceClient(ServiceKeys.VisionSubscriptionKey));
            SimpleIoc.Default.Register<EmotionServiceClient>(() => new EmotionServiceClient(ServiceKeys.EmotionSubscriptionKey));
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();

        public SettingsViewModel SettingsViewModel => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public AboutViewModel AboutViewModel => ServiceLocator.Current.GetInstance<AboutViewModel>();

        public static VisionServiceClient VisionServiceClient => ServiceLocator.Current.GetInstance<VisionServiceClient>();

        public static EmotionServiceClient EmotionServiceClient => ServiceLocator.Current.GetInstance<EmotionServiceClient>();

        public static ITranslatorService TranslatorService => ServiceLocator.Current.GetInstance<ITranslatorService>();

        static partial void OnInitialize();

        private static CultureInfo culture;
        public static string GetLanguage()
        {
            if (culture == null)
            {
#if __ANDROID__ || __IOS__
                var language = Localization.Resources.AppResources.ResourceLanguage;
#else
                var language = global::Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
#endif

                culture = new CultureInfo(language);
                if (!culture.IsNeutralCulture)
                    culture = culture.Parent;
            }

            return culture.TwoLetterISOLanguageName;
        }
    }
}
