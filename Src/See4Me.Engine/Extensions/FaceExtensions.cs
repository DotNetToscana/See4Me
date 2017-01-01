using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Face.Contract;

namespace See4Me.Engine.Extensions
{
    internal static class FaceExtensions
    {
        public static Rectangle[] ToRectangle(this Microsoft.ProjectOxford.Face.Contract.FaceRectangle faceRectangle)
            => new Rectangle[] {
                new Rectangle {
                    Height = faceRectangle.Height,
                    Left = faceRectangle.Left,
                    Top = faceRectangle.Top,
                    Width = faceRectangle.Width
                }
            };

        public static Emotion GetBestEmotion(this Microsoft.ProjectOxford.Emotion.Contract.Emotion[] emotions)
        {
            var bestEmotion = emotions.FirstOrDefault()?.Scores.ToRankedList().FirstOrDefault().Key;
            return (Emotion)Enum.Parse(typeof(Emotion), bestEmotion, true);
        }

        public static FaceResult GetFaceResult(this Face face)
        {
            return new FaceResult
            {
                Age = (int)face.FaceAttributes.Age,
                Gender = (Gender)Enum.Parse(typeof(Gender), face.FaceAttributes.Gender, true)
            };
        }
    }
}
