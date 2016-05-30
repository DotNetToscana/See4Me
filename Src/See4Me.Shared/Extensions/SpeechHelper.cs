using Microsoft.ProjectOxford.Vision.Contract;
using See4Me.Localization.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace See4Me.Extensions
{
    public static class SpeechHelper
    {
        public static string GetEmotionMessage(Face face, string bestEmotion)
        {
            // Creates the emotion description text to be speeched.
            string emotionMessage = null;

            var ageDescription = GetAgeDescription(face);
            var personAgeMessage = string.Format(GetString(Constants.PersonAgeMessage, face.Gender), face.Age, ageDescription);

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
    }
}
