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
        // or need to be temporaly removed (the latter will be probably added
        // again in a future release).
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
            "a blurry picture of",
            "a dark night",
            "a cat sitting in a dark room",
            "a laptop is lit up",
            "a cat that is lit up",
            "a man flying"
        };

        private static List<string> RemovedDescriptions = new List<string>
        {
            "holding a wine glass",
            "holding a basketball bat",
            "holding a remote control",
            "holding a donut",
            "eating a donut",
            "and a tie",
            "wearing a tie",
            "holding a wii remote",
            "brushing his teeth",
            "brushing her teeth",
            "with a remote",
            "the airplane is parked on the side of",
            "a picture of"
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
                    Text = !string.IsNullOrWhiteSpace(textToRemove) ? caption.Text.Replace(textToRemove, string.Empty).Trim() : caption.Text,
                    Confidence = caption.Confidence
                };

                return true;
            }

            return false;
        }
    }
}
