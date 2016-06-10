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
            var bestEmotion = scores.ToRankedList().FirstOrDefault().Key;
            if (bestEmotion != nameof(Scores.Neutral))
                return bestEmotion;

            return null;
        }
    }
}
