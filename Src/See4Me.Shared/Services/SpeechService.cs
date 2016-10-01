using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

namespace See4Me.Services
{
    public class SpeechService : ISpeechService
    {
        private readonly ITextToSpeech synthesizer;

        public SpeechService()
        {
            synthesizer = CrossTextToSpeech.Current;
            synthesizer.Init();
        }

        public string Language { get; set; }

        public Task SpeechAsync(string text, string language = null)
        {
            CrossLocale? locale = null;
            language = language ?? Language;

            if (language != null)
                locale = new CrossLocale { Language = language, Country = language };

            synthesizer.Speak(text, crossLocale: locale);
            return Task.FromResult<object>(null);
        }
    }
}
