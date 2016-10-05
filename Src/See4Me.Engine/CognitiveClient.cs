using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using See4Me.Engine.Extensions;
using See4Me.Engine.Services.ServiceSettings;
using See4Me.Engine.Services.Translator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine
{
    public class CognitiveClient
    {
        public CognitiveSettings Settings { get; } = new CognitiveSettings();

        public IVisionSettingsProvider VisionSettingsProvider { get; set; }

        private ITranslatorService translatorService;

        private const string DefaultLanguge = "en";

        public bool IsVisionServiceRegistered => !string.IsNullOrWhiteSpace(Settings.VisionSubscriptionKey);

        public bool IsEmotionServiceRegistered => !string.IsNullOrWhiteSpace(Settings.EmotionSubscriptionKey);

        public bool IsTranslatorServiceRegistered
            => !string.IsNullOrWhiteSpace(Settings.TranslatorClientId) && !string.IsNullOrWhiteSpace(Settings.TranslatorClientSecret);

        public CognitiveClient(IVisionSettingsProvider visionSettingsProvider = null)
        {
            VisionSettingsProvider = visionSettingsProvider;
        }

        public Task<CognitiveResult> RecognizeAsync(string language, byte[] buffer, RecognitionType recognitionType = RecognitionType.Vision | RecognitionType.Emotion, Func<RecognitionPhase, Task> onProgress = null)
            => RecognizeAsync(new MemoryStream(buffer), language, recognitionType, onProgress);

        public async Task<CognitiveResult> RecognizeAsync(Stream stream, string language, RecognitionType recognitionType = RecognitionType.Vision | RecognitionType.Emotion, Func<RecognitionPhase, Task> onProgress = null)
        {
            await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.QueryingService);

            var visionService = new VisionServiceClient(Settings.VisionSubscriptionKey);
            var result = new CognitiveResult();

            if (recognitionType.HasFlag(RecognitionType.Vision) || recognitionType.HasFlag(RecognitionType.Emotion))
            {
                var imageBytes = await stream.ToArrayAsync();

                var features = new HashSet<VisualFeature> { VisualFeature.Description };
                if (recognitionType.HasFlag(RecognitionType.Emotion))
                    features.Add(VisualFeature.Faces);

                var visionSettings = await VisionSettingsProvider?.GetSettingsAsync();
                var analyzeImageResult = await visionService.AnalyzeImageAsync(stream, features);
                var visionResult = result.VisionResult;

                Caption originalDescription;
                Caption filteredDescription;

                var isValid = analyzeImageResult.IsValid(out originalDescription, out filteredDescription, visionSettings);

                visionResult.IsValid = isValid;
                visionResult.RawDescription = originalDescription.Text;
                visionResult.Confidence = originalDescription.Confidence;

                if (isValid)
                {
                    visionResult.Description = filteredDescription.Text;

                    if (language != DefaultLanguge && IsTranslatorServiceRegistered)
                    {
                        if (Settings.TranslatorClientId != translatorService?.ClientId || Settings.TranslatorClientSecret != translatorService?.ClientSecret)
                            translatorService = new TranslatorService(Settings.TranslatorClientId, Settings.TranslatorClientSecret);

                        // The description needs to be translated.
                        await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.Translating);

                        var translation = await translatorService.TranslateAsync(filteredDescription.Text, from: DefaultLanguge, to: language);
                        visionResult.TranslatedDescription = translation;
                    }
                }

                if (recognitionType.HasFlag(RecognitionType.Emotion))
                {
                    // If there is one or more faces, asks the service information about them.
                    if (IsEmotionServiceRegistered && (analyzeImageResult.Faces?.Any() ?? false))
                    {
                        await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.RecognizingFaces);

                        var emotionService = new EmotionServiceClient(Settings.EmotionSubscriptionKey);

                        foreach (var face in analyzeImageResult.Faces)
                        {
                            using (var ms = new MemoryStream(imageBytes))
                            {
                                var emotions = await emotionService.RecognizeAsync(ms, face.FaceRectangle.ToRectangle());
                                var emotionResult = emotions.GetEmotionResult(face);
                                result.EmotionResults.Add(emotionResult);
                            }
                        }
                    }
                }
            }

            if (recognitionType.HasFlag(RecognitionType.Text))
            {
                await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.RecognizingText);

                var results = await visionService.RecognizeTextAsync(stream);
                var text = results.GetRecognizedText();

                result.OcrResult.Text = text;
            }

            return result;
        }

        private async Task RaiseOnProgressAsync(Func<RecognitionPhase, Task> onProgress, RecognitionPhase phase)
        {
            var handler = onProgress;
            if (handler != null)
                await handler.Invoke(phase);
        }
    }
}
