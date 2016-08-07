using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using See4Me.Services.ServiceSettings;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace See4Me.Extensions
{
    public static class VisionExtensions
    {
        private static VisionSettings settings;

        public static async Task InitializeAsync()
        {
            try
            {
                if (settings == null)
                {
                    var settingsProvider = ServiceLocator.Current.GetInstance<IVisionSettingsProvider>();
                    settings = await settingsProvider.GetSettingsAsync();
                }
            }
            catch { }
        }

        public static bool IsValid(this AnalysisResult result, out Caption rawDescription, out Caption filteredDescription)
        {
            rawDescription = result.Description.Captions.FirstOrDefault();
            filteredDescription = null;

            // If there is no settings, all the descriptions are valid.
            if (settings == null)
            {
                filteredDescription = rawDescription;
                return true;
            }

            if (rawDescription?.Confidence >= settings.MinimumConfidence)
            {
                var text = rawDescription.Text.ToLower();

                string replacedText = null;
                if (settings.DescriptionsToReplace.TryGetValue(text, out replacedText))
                    text = replacedText;

                var textToRemove = settings.DescriptionsToRemove.FirstOrDefault(d => text.Contains(d));
                var filteredText = !string.IsNullOrWhiteSpace(textToRemove) ? text.Replace(textToRemove, string.Empty).Trim() : text;

                if (!settings.InvalidDescriptions.Any(d => filteredText.Contains(d)))
                {
                    filteredDescription = new Caption
                    {
                        Text = filteredText,
                        Confidence = rawDescription.Confidence
                    };

                    return true;
                }
            }

            return false;
        }
    }
}
