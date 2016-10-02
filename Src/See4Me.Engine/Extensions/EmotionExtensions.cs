using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.ProjectOxford.Common;

namespace See4Me.Engine.Extensions
{
    internal static class EmotionExtensions
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

        public static Emotion GetBestEmotion(this Scores scores)
        {
            var bestEmotion = scores.ToRankedList().FirstOrDefault().Key;
            return (Emotion)Enum.Parse(typeof(Emotion), bestEmotion, true);
        }

        public static EmotionResult GetEmotionResult(this Microsoft.ProjectOxford.Emotion.Contract.Emotion[] emotions, Microsoft.ProjectOxford.Vision.Contract.Face face)
        {
            var bestEmotion = emotions.FirstOrDefault()?.Scores.GetBestEmotion();

            return new EmotionResult
            {
                Age = face.Age,
                Emotion = bestEmotion ?? Emotion.Neutral,
                Gender = (Gender)Enum.Parse(typeof(Gender), face.Gender, true)
            };
        }
    }
}
