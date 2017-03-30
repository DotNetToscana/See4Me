using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Common.Contract;

namespace See4Me.Engine.Extensions
{
    internal static class FaceExtensions
    {
        public static Emotion GetBestEmotion(this EmotionScores emotions)
        {
            var bestEmotion = emotions?.ToRankedList().FirstOrDefault().Key ?? nameof(EmotionScores.Neutral);
            return (Emotion)Enum.Parse(typeof(Emotion), bestEmotion, true);
        }

        public static FaceResult GetFaceResult(this Face face)
        {
            return new FaceResult
            {
                Age = (int)face.FaceAttributes.Age,
                Gender = (Gender)Enum.Parse(typeof(Gender), face.FaceAttributes.Gender, true),
                Emotion = face.FaceAttributes.Emotion.GetBestEmotion()
            };
        }
    }
}
