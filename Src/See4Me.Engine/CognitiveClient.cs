using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine
{
    public class CognitiveClient : IDisposable
    {
        private const string DefaultLanguge = "en";

        private readonly ITranslatorServiceClient translatorService;
        private readonly HttpClient httpClient;

        public CognitiveSettings Settings { get; set; }

        public IVisionSettingsProvider VisionSettingsProvider { get; set; }

        public bool IsVisionServiceRegistered => !string.IsNullOrWhiteSpace(Settings.VisionSubscriptionKey);

        public bool IsEmotionServiceRegistered => !string.IsNullOrWhiteSpace(Settings.EmotionSubscriptionKey);

        public bool IsFaceServiceRegistered => !string.IsNullOrWhiteSpace(Settings.FaceSubscriptionKey);

        public bool IsTranslatorServiceRegistered => !string.IsNullOrWhiteSpace(Settings.TranslatorSubscriptionKey);

        public CognitiveClient(CognitiveSettings settings = null, IVisionSettingsProvider visionSettingsProvider = null)
        {
            Settings = settings ?? new CognitiveSettings();
            VisionSettingsProvider = visionSettingsProvider;

            translatorService = new TranslatorServiceClient();
            httpClient = new HttpClient();
        }

        public async Task<CognitiveResult> AnalyzeAsync(string sourceUrl, string language, RecognitionType recognitionType = RecognitionType.All, Func<RecognitionPhase, Task> onProgress = null)
        {
            var buffer = await httpClient.GetByteArrayAsync(sourceUrl);
            return await this.AnalyzeAsync(buffer, language, recognitionType, onProgress);
        }

        public Task<CognitiveResult> AnalyzeAsync(byte[] buffer, string language, RecognitionType recognitionType = RecognitionType.All, Func<RecognitionPhase, Task> onProgress = null)
            => AnalyzeAsync(new MemoryStream(buffer), language, recognitionType, onProgress);

        public async Task<CognitiveResult> AnalyzeAsync(Stream stream, string language, RecognitionType recognitionType = RecognitionType.All, Func<RecognitionPhase, Task> onProgress = null)
        {
            var result = new CognitiveResult();

            var imageBytes = await stream.ToArrayAsync();
            await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.QueryingService);

            var visionService = new VisionServiceClient(Settings.VisionSubscriptionKey);
            AnalysisResult analyzeImageResult = null;

            if (recognitionType.HasFlag(RecognitionType.Vision))
            {
                var features = new HashSet<VisualFeature> { VisualFeature.Description };

                // If recognition types include face or emotions, adde also the Face Visual Feature, so Face and Emotion services are called
                // only if really needed.
                if (recognitionType.HasFlag(RecognitionType.Face) || recognitionType.HasFlag(RecognitionType.Emotion))
                    features.Add(VisualFeature.Faces);

                try
                {
                    analyzeImageResult = await visionService.AnalyzeImageAsync(stream, features);
                }
                catch (Microsoft.ProjectOxford.Vision.ClientException ex)
                {
                    var exception = await this.CreateExceptionAsync(ex.Error.Code, ex.Error.Message, "Vision", ex.GetHttpStatusCode(), ex, language, onProgress);
                    throw exception;
                }

                Caption originalDescription;
                Caption filteredDescription;
                var visionSettings = VisionSettingsProvider != null ? await VisionSettingsProvider.GetSettingsAsync() : null;
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
            }

            if ((recognitionType.HasFlag(RecognitionType.Face) || recognitionType.HasFlag(RecognitionType.Emotion))
                && (analyzeImageResult?.Faces.Any() ?? true))   // If Vision service was previously called, checks if any face was detected.
            {
                var faceService = new FaceServiceClient(Settings.FaceSubscriptionKey);
                var emotionService = new EmotionServiceClient(Settings.EmotionSubscriptionKey);

                await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.RecognizingFaces);

                try
                {
                    stream.Position = 0;
                    var faces = await faceService.DetectAsync(stream, returnFaceAttributes: new[] { FaceAttributeType.Gender, FaceAttributeType.Age /*, FaceAttributeType.Smile, FaceAttributeType.Glasses */ });

                    if (faces.Any())
                    {
                        // Tries to identify faces in the image.
                        IdentifyResult[] faceIdentificationResult = null;
                        var personGroups = await faceService.ListPersonGroupsAsync();
                        var defaultPersonGroupId = (personGroups.FirstOrDefault(p => p.Name.ContainsIgnoreCase("See4Me") || p.UserData.ContainsIgnoreCase("See4Me") || p.Name.ContainsIgnoreCase("_default") || p.UserData.ContainsIgnoreCase("_default")) ?? personGroups.FirstOrDefault())?.PersonGroupId;

                        if (!string.IsNullOrWhiteSpace(defaultPersonGroupId))
                        {
                            var faceIds = faces.Select(face => face.FaceId).ToArray();
                            faceIdentificationResult = await faceService.IdentifyAsync(defaultPersonGroupId, faceIds);
                        }

                        foreach (var face in faces)
                        {
                            var faceResult = face.GetFaceResult();

                            // Checks if there is a candidate (i.e. a known person) in the identification result.
                            var candidate = faceIdentificationResult?.FirstOrDefault(r => r.FaceId == face.FaceId)?.Candidates.FirstOrDefault();
                            if (candidate != null)
                            {
                                var person = await faceService.GetPersonAsync(defaultPersonGroupId, candidate.PersonId);
                                faceResult.IdentifyConfidence = candidate.Confidence;
                                faceResult.Name = person?.Name;
                            }

                            if (recognitionType.HasFlag(RecognitionType.Emotion))
                            {
                                // If required, for each face get the corresponding emotion.
                                try
                                {
                                    using (var ms = new MemoryStream(imageBytes))
                                    {
                                        var emotions = await emotionService.RecognizeAsync(ms, face.FaceRectangle.ToRectangle());
                                        var bestEmotion = emotions.GetBestEmotion();

                                        faceResult.Emotion = bestEmotion;
                                    }
                                }
                                catch (Microsoft.ProjectOxford.Common.ClientException ex)
                                {
                                    var exception = await this.CreateExceptionAsync(ex.Error.Code, ex.Error.Message, "Emotion", ex.HttpStatus, ex, language, onProgress);
                                    throw exception;
                                }
                            }

                            result.FaceResults.Add(faceResult);
                        }
                    }
                }
                catch (FaceAPIException ex)
                {
                    var exception = await this.CreateExceptionAsync(ex.ErrorCode, ex.ErrorMessage, "Face", ex.HttpStatus, ex, language, onProgress);
                    throw exception;
                }
            }

            if (recognitionType.HasFlag(RecognitionType.Text))
            {
                await this.RaiseOnProgressAsync(onProgress, RecognitionPhase.RecognizingText);

                try
                {
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        var results = await visionService.RecognizeTextAsync(ms);
                        var text = results.GetRecognizedText();
                        result.OcrResult.Text = text;
                    }
                }
                catch (Microsoft.ProjectOxford.Vision.ClientException ex)
                {
                    var exception = await this.CreateExceptionAsync(ex.Error.Code, ex.Error.Message, "Vision", ex.GetHttpStatusCode(), ex, language, onProgress);
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

        private async Task<CognitiveException> CreateExceptionAsync(string code, string message, string source, HttpStatusCode statusCode, Exception originalException, string language, Func<RecognitionPhase, Task> onProgress = null)
        {
            try
            {
                message = await this.TranslateAsync(message, language, onProgress);
            }
            catch { }

            var exception = new CognitiveException(message, originalException)
            {
                Code = code,
                HttpStatusCode = (statusCode == 0 ? HttpStatusCode.InternalServerError : statusCode),
                Source = source
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

        public void Dispose()
        {
            translatorService.Dispose();
            httpClient.Dispose();
        }
    }
}
