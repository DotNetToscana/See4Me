using Microsoft.Practices.ServiceLocation;
using Microsoft.ProjectOxford.Vision.Contract;
using See4Me.Localization.Resources;
using See4Me.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace See4Me.Extensions
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

        public static string GetEmotionMessage(Face face, string bestEmotion, bool includeAge)
        {
            //if (bestEmotion == null && !includeAge)
            //{
            //    // If no emotion recognized and we don't want to speech age, we actually don't have anything.
            //    return null;
            //}

            // Creates the emotion description text to be speeched.
            string personAgeMessage = null;
            string emotionMessage = null;

            var ageDescription = GetAgeDescription(face);

            if (includeAge)
                personAgeMessage = string.Format(GetString(Constants.PersonAgeMessage, face.Gender), ageDescription, face.Age);
            else
                personAgeMessage = string.Format(GetString(Constants.PersonMessage, face.Gender), ageDescription);

            if (bestEmotion != null)
            {
                var emotion = GetString(bestEmotion, face.Gender);
                var lookingMessage = string.Format(GetString(Constants.LookingMessage, face.Gender), emotion);
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

        private static string GetAgeDescription(Face face)
        {
            string key = null;

            if (face.Age <= 13)
                key = Constants.Child;
            else if (face.Age >= 14 && face.Age <= 29)
                key = Constants.Boy;
            else
                key = Constants.Man;

            var ageDescription = GetString(key, face.Gender);
            return ageDescription;
        }

        private static string GetString(string key, string gender)
            => AppResources.ResourceManager.GetString(key + gender);

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
