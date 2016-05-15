using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.ProjectOxford.Common;

namespace See4Me.Extensions
{
    public static class EmotionExtensions
    {
        public static Rectangle[] ToRectangle(this Microsoft.ProjectOxford.Vision.Contract.FaceRectangle faceRectangle)
            => new Rectangle[] {
                new Rectangle {
                    Height = faceRectangle.Height,
                    Left = faceRectangle.Left,
                    Top = faceRectangle.Top,
                    Width = faceRectangle.Width
                }
            };

        public static string GetBestEmotion(this Scores scores)
        {
            var list = new Dictionary<string, float>
            {
                [nameof(Scores.Anger)] = scores.Anger,
                [nameof(Scores.Contempt)] = scores.Contempt,
                [nameof(Scores.Disgust)] = scores.Disgust,
                [nameof(Scores.Fear)] = scores.Fear,
                [nameof(Scores.Happiness)] = scores.Happiness,
                [nameof(Scores.Neutral)] = scores.Neutral,
                [nameof(Scores.Sadness)] = scores.Sadness,
                [nameof(Scores.Surprise)] = scores.Surprise,
            };

            var bestEmotion = list.FirstOrDefault(x => x.Value == list.Values.Max()).Key;
            if (bestEmotion != nameof(Scores.Neutral))
                return bestEmotion;

            return null;
        }
    }
}
