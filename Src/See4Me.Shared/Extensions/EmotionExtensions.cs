using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace See4Me.Extensions
{
    public static class EmotionExtensions
    {
        public static Microsoft.ProjectOxford.Common.Rectangle[] ToRectangle(this FaceRectangle faceRectangle)
            => new Microsoft.ProjectOxford.Common.Rectangle[] {
                new Microsoft.ProjectOxford.Common.Rectangle {
                    Height = faceRectangle.Height,
                    Left = faceRectangle.Left,
                    Top = faceRectangle.Top,
                    Width = faceRectangle.Width }
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

            return null;
        }
    }
}
