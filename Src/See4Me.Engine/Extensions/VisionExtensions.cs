using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using See4Me.Engine.Services.ServiceSettings;

namespace See4Me.Engine.Extensions
{
    internal static class VisionExtensions
    {
        public static bool IsValid(this AnalysisResult result, out Caption rawDescription, out Caption filteredDescription, VisionSettings settings = null)
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
