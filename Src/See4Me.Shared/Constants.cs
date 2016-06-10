namespace See4Me
{
    public static partial class Constants
    {
        public const string DefaultLanguge = "en";

        public const string MainPage = "MainPage";
        public const string TakePhoto = "TakePhoto";
        public const string InitializeStreaming = "InitializeStreaming";

        public const string Child = "Child";
        public const string Boy = "Boy";
        public const string Man = "Man";
        public const string LookingMessage = "Looking";
        public const string PersonAgeMessage = "PersonAgeMessage";
        public const string PersonMessage = "PersonMessage";
        public const string EmotionMessage = "EmotionMessage";

#if __ANDROID__
        public const string SentenceEnd = " / ";
#elif __IOS__
        public const string SentenceEnd = ".";
#else
        public const string SentenceEnd = "..";
#endif
    }
}
