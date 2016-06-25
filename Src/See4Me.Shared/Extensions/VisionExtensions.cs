using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace See4Me.Extensions
{
    public static class VisionExtensions
    {
        private const double MINIMUM_CONFIDENCE = 0.16d;

        // Somes strings represents a description that is actually invalid
        // or just need to be removed.
        private static List<string> InvalidDescriptions = new List<string>
        {
            "a bird that is lit up at night",
            "person sitting at night",
            "a dark room",
            "a close up of",
            "a picture of a sign",
            "a room with a remote control",
            "a reflection in a mirror",
            "a blurry photo of",
            "a blurry picture of"
        };

        private static List<string> RemovedDescriptions = new List<string>
        {
            "holding a wine glass",
            "holding a basketball bat",
            "holding a remote control",
            "holding a donut",
            "and a tie",
            "wearing a tie",
            "holding a wii remote",
        };

        public static bool IsValid(this AnalysisResult result, out Caption description)
        {
            description = null;
            var caption = result.Description.Captions.FirstOrDefault();

            if (caption != null && caption.Confidence >= MINIMUM_CONFIDENCE && !InvalidDescriptions.Any(d => caption.Text.StartsWith(d)))
            {
                var textToRemove = RemovedDescriptions.FirstOrDefault(d => caption.Text.Contains(d));

                description = new Caption
                {
                    Text = !string.IsNullOrWhiteSpace(textToRemove) ? caption.Text.Replace(textToRemove, string.Empty) : caption.Text,
                    Confidence = caption.Confidence
                };

                return true;
            }

            return false;
        }
    }
}
