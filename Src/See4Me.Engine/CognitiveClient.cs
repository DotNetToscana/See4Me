using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using See4Me.Engine.Extensions;
using See4Me.Engine.Services.ServiceSettings;
using See4Me.Engine.Services.TranslatorService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine
{
    public class CognitiveClient
    {
        private const string DefaultLanguge = "en";

        private readonly ITranslatorServiceClient translatorService;

        public CognitiveSettings Settings { get; set; } = new CognitiveSettings();

        public IVisionSettingsProvider VisionSettingsProvider { get; set; }

        public bool IsVisionServiceRegistered => !string.IsNullOrWhiteSpace(Settings.VisionSubscriptionKey);

        public bool IsEmotionServiceRegistered => !string.IsNullOrWhiteSpace(Settings.EmotionSubscriptionKey);

        public bool IsTranslatorServiceRegistered => !string.IsNullOrWhiteSpace(Settings.TranslatorSubscriptionKey);

        public CognitiveClient(IVisionSettingsProvider visionSettingsProvider = null)
        {
            VisionSettingsProvider = visionSettingsProvider;
            translatorService = new TranslatorServiceClient();
        }

        public Task<CognitiveResult> AnalyzeAsync(byte[] buffer, string language, RecognitionType recognitionType = RecognitionType.Vision | RecognitionType.Emotion, Func<RecognitionPhase, Task> onProgress = null)
            => AnalyzeAsync(new MemoryStream(buffer), language, recognitionType, onProgress);

        public async Task<CognitiveResult> AnalyzeAsync(Stream stream, string language, RecognitionType recognitionType = RecognitionType.Vision | RecognitionType.Emotion, Func<RecognitionPhase, Task> onProgress = null)
        {
            var result = new CognitiveResult();
            await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.QueryingService);

            var visionService = new VisionServiceClient(Settings.VisionSubscriptionKey);

            if (recognitionType.HasFlag(RecognitionType.Vision) || recognitionType.HasFlag(RecognitionType.Emotion))
            {
                var imageBytes = await stream.ToArrayAsync();

                var features = new HashSet<VisualFeature> { VisualFeature.Description };
                if (recognitionType.HasFlag(RecognitionType.Emotion))
                    features.Add(VisualFeature.Faces);

                AnalysisResult analyzeImageResult = null;
                VisionSettings visionSettings = null;

                try
                {
                    analyzeImageResult = await visionService.AnalyzeImageAsync(stream, features);
                }
                catch (Microsoft.ProjectOxford.Vision.ClientException ex)
                {
                    var exception = await this.CreateExceptionAsync(ex.Error.Code, ex.Error.Message, ex.GetHttpStatusCode(), ex, language, onProgress);
                    throw exception;
                }

                if (VisionSettingsProvider != null)
                    visionSettings = await VisionSettingsProvider.GetSettingsAsync();

                Caption originalDescription;
                Caption filteredDescription;
                var isValid = analyzeImageResult.IsValid(out originalDescription, out filteredDescription, visionSettings);

                var visionResult = result.VisionResult;
                visionResult.IsValid = isValid;
                visionResult.RawDescription = originalDescription.Text;
                visionResult.Confidence = originalDescription.Confidence;

                if (isValid)
                {
                    visionResult.Description = filteredDescription.Text;
                    visionResult.TranslatedDescription = await this.TranslateAsync(filteredDescription.Text, language, onProgress);
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
                                try
                                {
                                    var emotions = await emotionService.RecognizeAsync(ms, face.FaceRectangle.ToRectangle());
                                    var emotionResult = emotions.GetEmotionResult(face);
                                    result.EmotionResults.Add(emotionResult);
                                }
                                catch (Microsoft.ProjectOxford.Common.ClientException ex)
                                {
                                    var exception = await this.CreateExceptionAsync(ex.Error.Code, ex.Error.Message, ex.HttpStatus, ex, language, onProgress);
                                    throw exception;
                                }
                            }
                        }
                    }
                }
            }

            if (recognitionType.HasFlag(RecognitionType.Text))
            {
                await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.RecognizingText);

                try
                {
                    var results = await visionService.RecognizeTextAsync(stream);
                    var text = results.GetRecognizedText();
                    result.OcrResult.Text = text;
                }
                catch (Microsoft.ProjectOxford.Vision.ClientException ex)
                {
                    var exception = await this.CreateExceptionAsync(ex.Error.Code, ex.Error.Message, ex.HttpStatus, ex, language, onProgress);
                    throw exception;
                }
            }

            return result;
        }

        private async Task RaiseOnProgressAsync(Func<RecognitionPhase, Task> onProgress, RecognitionPhase phase)
        {
            var handler = onProgress;
            if (handler != null)
                await handler.Invoke(phase);
        }

        private async Task<CognitiveException> CreateExceptionAsync(string code, string message, HttpStatusCode statusCode, Exception originalException, string language, Func<RecognitionPhase, Task> onProgress = null)
        {
            try
            {
                message = await this.TranslateAsync(message, language, onProgress);
            }
            catch { }

            var exception = new CognitiveException(message, originalException)
            {
                Code = code,
                HttpStatusCode = statusCode
            };

            return exception;
        }

        private async Task<string> TranslateAsync(string message, string language, Func<RecognitionPhase, Task> onProgress = null)
        {
            var translation = message;
            if (!string.IsNullOrWhiteSpace(language) && language != DefaultLanguge && IsTranslatorServiceRegistered)
            {
                // Make sure to use the updated translator subscription key.
                translatorService.SubscriptionKey = Settings.TranslatorSubscriptionKey;

                // The description needs to be translated.
                await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.Translating);

                translation = await translatorService.TranslateAsync(message, from: DefaultLanguge, to: language);
            }

            return translation;
        }
    }
}
