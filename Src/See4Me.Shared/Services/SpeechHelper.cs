using Microsoft.Practices.ServiceLocation;
using Microsoft.ProjectOxford.Vision.Contract;
using See4Me.Engine;
using See4Me.Localization.Resources;
using See4Me.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public static class SpeechHelper
    {
        private static readonly ISettingsService settings;
        private static readonly ISpeechService speechService;

        static SpeechHelper()
        {
            settings = ServiceLocator.Current.GetInstance<ISettingsService>();
            speechService = ServiceLocator.Current.GetInstance<ISpeechService>();
        }

        public static string GetEmotionMessage(Gender gender, int age, Emotion bestEmotion)
        {
            // Creates the emotion description text to be speeched.
            string emotionMessage = null;

            var ageDescription = GetAgeDescription(age, gender);
            var personAgeMessage = string.Format(GetString(Constants.PersonAgeMessage, gender), ageDescription, age);

            if (bestEmotion != Emotion.Neutral)
            {
                var emotion = GetString(bestEmotion.ToString(), gender);
                var lookingMessage = string.Format(GetString(Constants.LookingMessage, gender), emotion);
                emotionMessage = $"{personAgeMessage} {lookingMessage}";
            }
            else
            {
                // No emotion recognized, so includes only the age in the message.
                emotionMessage = personAgeMessage;
            }

            emotionMessage = $"{emotionMessage} {Constants.SentenceEnd} ";
            return emotionMessage;
        }

        private static string GetAgeDescription(int age, Gender gender)
        {
            string key = null;

            if (age <= 13)
                key = Constants.Child;
            else if (age >= 14 && age <= 29)
                key = Constants.Boy;
            else
                key = Constants.Man;

            var ageDescription = GetString(key, gender);
            return ageDescription;
        }

        private static string GetString(string key, Gender gender)
            => AppResources.ResourceManager.GetString(key + gender.ToString());

        public static async Task TrySpeechAsync(string message)
        {
            if (settings.IsTextToSpeechEnabled)
            {
                var speechMessage = Regex.Replace(message, @" ?\(.*?\)", string.Empty);
                await speechService.SpeechAsync(speechMessage);
            }
        }
    }
}
