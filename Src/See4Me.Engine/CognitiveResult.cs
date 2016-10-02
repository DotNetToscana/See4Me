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
    }

    public class VisionResult
    {
        public bool IsValid { get; set; }

        public double Confidence { get; set; }

        public string Description { get; set; }

        private string translatedDescription;
        public string TranslatedDescription
        {
            get { return translatedDescription ?? Description; }
            set { translatedDescription = value; }
        }

        public bool IsTranslated => Description != translatedDescription;

        public string RawDescription { get; set; }
    }

    public class EmotionResult
    {
        public Emotion Emotion { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }
    }

    public class OcrResult
    {
        public string Text { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(Text);
    }
}
