using Microsoft.Practices.ServiceLocation;
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

        public static FaceResultMessage GetFaceMessage(FaceResult face)
        {
            var result = new FaceResultMessage();

            // Creates the face description text to be speeched.
            string faceMessage = null;
            string personMessage;

            if (!string.IsNullOrWhiteSpace(face.Name))
            {
                // A person name has been identified.
                personMessage = $"{face.Name} ";

                if (settings.ShowRecognitionConfidence)
                { 
                    personMessage = $"{personMessage} ({Math.Round(face.IdentifyConfidence, 2)})";
                }

                result.ContainsFace = true;
            }
            else
            {
                var ageDescription = GetAgeDescription(face.Age, face.Gender);
                personMessage = string.Format(GetString(Constants.PersonAgeMessage, face.Gender), ageDescription, face.Age);
            }

            if (face.Emotion != Emotion.Neutral)
            {
                var emotion = GetString(face.Emotion.ToString(), face.Gender).ToLower();
                var lookingMessage = string.Format(GetString(Constants.LookingMessage, face.Gender), emotion);
                faceMessage = $"{personMessage} {lookingMessage}";
            }
            else
            {
                // No emotion recognized, so includes only the person name or age in the message.
                faceMessage = personMessage;
            }

            faceMessage = $"{faceMessage} {Constants.SentenceEnd} ";
            result.Message = faceMessage;

            return result;
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

            var ageDescription = GetString(key, gender).ToLower();
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

    public class FaceResultMessage
    {
        public bool ContainsFace { get; set; }

        public string Message { get; set; }
    }
}
