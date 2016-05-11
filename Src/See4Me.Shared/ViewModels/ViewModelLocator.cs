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

            SimpleIoc.Default.Register<ITranslatorService>(() =>
            {
                var language = GetLanguage();
                var service = new TranslatorService(ServiceKeys.TranslatorClientId, ServiceKeys.TranslatorClientSecret, language);

                return service;
            });

            SimpleIoc.Default.Register<VisionServiceClient>(() => new VisionServiceClient(ServiceKeys.VisionSubscriptionKey));
            SimpleIoc.Default.Register<EmotionServiceClient>(() => new EmotionServiceClient(ServiceKeys.EmotionSubscriptionKey));

            SimpleIoc.Default.Register<ISpeechService, SpeechService>();
            SimpleIoc.Default.Register<IStreamingService, StreamingService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();

            SimpleIoc.Default.Register<MainViewModel>();

            OnInitialize();
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();

        static partial void OnInitialize();

        private static CultureInfo culture;
        public static string GetLanguage()
        {
            if (culture == null)
            {
#if __ANDROID__ || __IOS__
                var language = See4Me.Localization.Resources.AppResources.ResourceLanguage;
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
