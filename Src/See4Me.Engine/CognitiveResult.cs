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

        public IList<EmotionResult> EmotionResults { get; internal set; } = new List<EmotionResult>();

        public OcrResult OcrResult { get; internal set; } = new OcrResult();

        internal CognitiveResult() { }
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

        internal VisionResult() { }
    }

    public class EmotionResult
    {
        public Emotion Emotion { get; internal set; }

        public int Age { get; internal set; }

        public Gender Gender { get; internal set; }

        internal EmotionResult() { }
    }

    public class OcrResult
    {
        public string Text { get; internal set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(Text);

        internal OcrResult() { }
    }
}
