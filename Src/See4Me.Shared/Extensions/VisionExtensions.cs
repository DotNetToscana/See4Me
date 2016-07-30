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

        /* Somes strings are actually invalid or need to be temporaly removed
           because very often they aren't so accurate
           (they will be probably added again in a future release).
        */

        private static List<string> InvalidDescriptions = new List<string>
        {
            "a bird that is lit up at night",
            "person sitting at night",
            "a picture of a sign",
            "a room with a remote control",
            "a reflection in a mirror",
            "a dark night",
            "a fire hydrant",
            "the dark",
            "a airplane that is on display",
            "an image of a bird",
            "a laptop is lit up on the screen",
            "himself in the mirror",
            "a blurry picture",
            "a cat that is lit up",
            "a dog that is lit up",
            "a bird that is lit up"
        };

        private static List<string> RemovedDescriptions = new List<string>
        {
            "holding a wine glass",
            "holding a basketball bat",
            "holding a remote control",
            "holding a donut",
            "eating a donut",
            "and a tie",
            "and tie",
            "wearing a tie",
            "holding a wii remote",
            "brushing his teeth",
            "brushing her teeth",
            "with a remote",
            "the airplane is parked on the side of",
            "a picture of",
            "a view of",
            "a blurry photo of",
            "a blurry picture of",
            "a bird flying in",
            "a bird standing on the side of",
            "a bird is standing near",
            "a train is parked on the side of",
            "a man flying in",
            "a airplane that is flying in",
            "a plane flying in",
            "a close up of",
            "a bird walikng on",
            "a man laying on",
            "a white toilet sitting in",
            "a man riding a skateboard up",
            "a hot dog in",
            "playing a video game",
            "a street scene with focus on",
            "a train that is on",
            "a man flying through",
            "a plane that is flying in",
            "a bird flying over",
            "the air while riding a skateboard on",
            "a bird walking on",
            "a clock tower in front of",
            "a clock sitting in front of",
            "a sign with a clock on"
        };

        public static bool IsValid(this AnalysisResult result, out Caption rawDescription, out Caption filteredDescription)
        {
            filteredDescription = null;
            rawDescription = result.Description.Captions.FirstOrDefault();

            if (rawDescription?.Confidence >= MINIMUM_CONFIDENCE)
            {
                var rawText = rawDescription.Text.ToLower();

                var textToRemove = RemovedDescriptions.FirstOrDefault(d => rawText.Contains(d));
                var filteredText = !string.IsNullOrWhiteSpace(textToRemove) ? rawText.Replace(textToRemove, string.Empty).Trim() : rawText;

                if (!InvalidDescriptions.Any(d => filteredText.Contains(d)))
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
