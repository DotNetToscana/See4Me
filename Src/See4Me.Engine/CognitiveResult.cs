using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine
{
    public class CognitiveResult
    {
        public VisionResult VisionResult { get; internal set; } = new VisionResult();

        public IList<FaceResult> FaceResults { get; internal set; } = new List<FaceResult>();

        public OcrResult OcrResult { get; internal set; } = new OcrResult();

        internal CognitiveResult()
        {
        }
    }

    public class VisionResult
    {
        public bool IsValid { get; internal set; }

        public double Confidence { get; internal set; }

        public string RawDescription { get; internal set; }

        public string Description { get; internal set; }

        private string translatedDescription;
        public string TranslatedDescription
        {
            get { return translatedDescription ?? Description; }
            internal set { translatedDescription = value; }
        }

        public bool IsTranslated => Description != translatedDescription;

        internal VisionResult()
        {
        }
    }

    public class FaceResult
    {
        public Emotion Emotion { get; internal set; } = Emotion.Neutral;

        public int Age { get; internal set; }

        public Gender Gender { get; internal set; }

        public string Name { get; internal set; }

        public double IdentifyConfidence { get; internal set; }

        internal FaceResult()
        {
        }
    }

    public class OcrResult
    {
        private string text;
        public string Text
        {
            get { return !string.IsNullOrWhiteSpace(text) ? text : null; }
            internal set { text = value; }
        }

        public bool ContainsText => !string.IsNullOrWhiteSpace(Text);

        internal OcrResult()
        {
        }
    }
}