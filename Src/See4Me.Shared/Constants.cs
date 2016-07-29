namespace See4Me
{
    public static partial class Constants
    {
        public const string DefaultLanguge = "en";

        public const string TakingPhoto = "TakingPhoto";
        public const string PhotoTaken = "PhotoTaken";
        public const string InitializeStreaming = "InitializeStreaming";

        public const string Child = "Child";
        public const string Boy = "Boy";
        public const string Man = "Man";
        public const string LookingMessage = "Looking";
        public const string PersonAgeMessage = "PersonAgeMessage";
        public const string PersonMessage = "PersonMessage";
        public const string EmotionMessage = "EmotionMessage";

        public const string CognitiveServicesUrl = "https://www.microsoft.com/cognitive-services";
        public const string MicrosoftPrivacyPoliciesUrl = "https://go.microsoft.com/fwlink/?LinkId=521839";
        public const string GitHubProjectUrl = "https://github.com/DotNetToscana/see4me";
        public const string CognitiveServicesSubscriptionUrl = "https://www.microsoft.com/cognitive-services/en-us/sign-up";
        public const string ActivateTranslatorServiceUrl = "https://datamarket.azure.com/dataset/bing/microsofttranslator";
        public const string TranslatorServiceCreateAppUrl = "https://datamarket.azure.com/developer/applications";
        public const string BlogUrl = "https://marcominerva.wordpress.com";
        public const string TwitterUrl = "https://twitter.com/marcominerva";
        public const string LinkedInUrl = "https://www.linkedin.com/in/marcominerva";

#if __ANDROID__
        public const string SentenceEnd = " / ";
#elif __IOS__
        public const string SentenceEnd = ".";
#else
        public const string SentenceEnd = "..";
#endif
    }
}
