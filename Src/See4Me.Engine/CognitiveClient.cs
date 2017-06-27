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

        private const string Landmarks = "Landmarks";
        private const string Celebrities = "Celebrieties";

        private readonly ITranslatorServiceClient translatorService;

        // Variables to handle face identification.
        private string identifyPersonGroupId = null;
        private bool faceServiceInitialized = false;

        public CognitiveSettings Settings { get; set; }

        public IVisionSettingsProvider VisionSettingsProvider { get; set; }

        public bool IsVisionServiceRegistered => !string.IsNullOrWhiteSpace(Settings.VisionSubscriptionKey);

        public bool IsFaceServiceRegistered => !string.IsNullOrWhiteSpace(Settings.FaceSubscriptionKey);

        public bool IsTranslatorServiceRegistered => !string.IsNullOrWhiteSpace(Settings.TranslatorSubscriptionKey);

        public CognitiveClient()
            : this(null, null)
        {
        }

        public CognitiveClient(IVisionSettingsProvider visionSettingsProvider)
            : this(null, visionSettingsProvider)
        {
        }

        public CognitiveClient(CognitiveSettings settings, IVisionSettingsProvider visionSettingsProvider)
        {
            Settings = settings ?? new CognitiveSettings();
            VisionSettingsProvider = visionSettingsProvider;

            translatorService = new TranslatorServiceClient();
        }

        public Task<CognitiveResult> AnalyzeAsync(byte[] buffer, string language, RecognitionType recognitionType = RecognitionType.All, Func<RecognitionPhase, Task> onProgress = null)
            => AnalyzeAsync(new MemoryStream(buffer), language, recognitionType, onProgress);

        public async Task<CognitiveResult> AnalyzeAsync(Stream stream, string language, RecognitionType recognitionType = RecognitionType.All, Func<RecognitionPhase, Task> onProgress = null)
        {
            var result = new CognitiveResult();

            var imageBytes = await stream.ToArrayAsync();
            await RaiseOnProgressAsync(onProgress, RecognitionPhase.QueryingService);

            var visionService = new VisionServiceClient(Settings.VisionSubscriptionKey);
            AnalysisResult analyzeImageResult = null;

            if (recognitionType.HasFlag(RecognitionType.Vision))
            {
                var features = new HashSet<VisualFeature> { VisualFeature.Description };
                var details = new HashSet<string> { Landmarks };

                if (recognitionType.HasFlag(RecognitionType.Face) || recognitionType.HasFlag(RecognitionType.Emotion))
                {
                    // If recognition types include face or emotions, add also the Faces Visual Feature, so Face and Emotion services are called
                    // only if really needed.
                    features.Add(VisualFeature.Faces);
                }

                try
                {
                    analyzeImageResult = await visionService.AnalyzeImageAsync(stream, features, details);
                }
                catch (ClientException ex)
                {
                    var exception = await CreateExceptionAsync(ex.Error.Code, ex.Error.Message, "Vision", ex.GetHttpStatusCode(), ex, language, onProgress);
                    throw exception;
                }

                var visionSettings = VisionSettingsProvider != null ? await VisionSettingsProvider.GetSettingsAsync() : null;
                var isValid = analyzeImageResult.IsValid(out var originalDescription, out var filteredDescription, visionSettings);

                var visionResult = result.VisionResult;
                visionResult.IsValid = isValid;
                visionResult.RawDescription = originalDescription.Text;
                visionResult.Confidence = originalDescription.Confidence;

                if (isValid)
                {
                    visionResult.Description = filteredDescription.Text;
                    visionResult.TranslatedDescription = await TranslateAsync(filteredDescription.Text, language, onProgress);
                }
            }

            if ((recognitionType.HasFlag(RecognitionType.Face) || recognitionType.HasFlag(RecognitionType.Emotion))
                && (analyzeImageResult?.Faces.Any() ?? true))   // If Vision service was previously called, checks if any face was detected.
            {
                var faceService = new FaceServiceClient(Settings.FaceSubscriptionKey);

                await RaiseOnProgressAsync(onProgress, RecognitionPhase.RecognizingFaces);

                try
                {
                    stream.Position = 0;
                    var attributes = new HashSet<FaceAttributeType> { FaceAttributeType.Gender, FaceAttributeType.Age };

                    if (recognitionType.HasFlag(RecognitionType.Emotion))
                    {
                        // If recognition types include emotions, add also the Emotion Face Attribute Type, so this feature is called
                        // only if really needed.
                        attributes.Add(FaceAttributeType.Emotion);
                    }

                    var faces = await faceService.DetectAsync(stream, returnFaceAttributes: attributes);

                    if (faces.Any())
                    {
                        if (!faceServiceInitialized)
                        {
                            // If necessary, initializes face service by obtaining the face group used for identification, if any.
                            await InitializeFaceServiceAsync(faceService);
                        }

                        // Tries to identify faces in the image.
                        IdentifyResult[] faceIdentificationResult = null;

                        if (!string.IsNullOrWhiteSpace(identifyPersonGroupId))
                        {
                            var faceIds = faces.Select(face => face.FaceId).ToArray();
                            faceIdentificationResult = await faceService.IdentifyAsync(identifyPersonGroupId, faceIds);
                        }

                        var faceTasks = new List<Task>();

                        foreach (var face in faces)
                        {
                            await RaiseOnProgressAsync(onProgress, RecognitionPhase.RecognizingFaces);

                            // Runs face identification in parallel.
                            var task = Task.Run(async () =>
                            {
                                var faceResult = face.GetFaceResult();

                                // Checks if there is a candidate (i.e. a known person) in the identification result.
                                var candidate = faceIdentificationResult?.FirstOrDefault(r => r.FaceId == face.FaceId)?.Candidates.FirstOrDefault();
                                if (candidate != null)
                                {
                                    // Gets the person name.
                                    var person = await faceService.GetPersonAsync(identifyPersonGroupId, candidate.PersonId);
                                    faceResult.IdentifyConfidence = candidate.Confidence;
                                    faceResult.Name = person?.Name;
                                }

                                result.FaceResults.Add(faceResult);
                            });

                            faceTasks.Add(task);
                        }

                        await Task.WhenAll(faceTasks);
                    }
                }
                catch (FaceAPIException ex)
                {
                    var exception = await CreateExceptionAsync(ex.ErrorCode, ex.ErrorMessage, "Face", ex.HttpStatus, ex, language, onProgress);
                    throw exception;
                }
            }

            if (recognitionType.HasFlag(RecognitionType.Text))
            {
                await RaiseOnProgressAsync(onProgress, RecognitionPhase.RecognizingText);

                try
                {
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        var results = await visionService.RecognizeTextAsync(ms);
                        var text = results.GetRecognizedText();
                        result.OcrResult.Text = text;
                    }
                }
                catch (ClientException ex)
                {
                    var exception = await CreateExceptionAsync(ex.Error.Code, ex.Error.Message, "Vision", ex.GetHttpStatusCode(), ex, language, onProgress);
                    throw exception;
                }
            }

            return result;
        }

        private async Task InitializeFaceServiceAsync(FaceServiceClient faceService)
        {
            try
            {
                var personGroups = await faceService.ListPersonGroupsAsync();
                identifyPersonGroupId = (personGroups.FirstOrDefault(p => p.Name.ContainsIgnoreCase("See4Me") || p.UserData.ContainsIgnoreCase("See4Me") || p.Name.ContainsIgnoreCase("_default") || p.UserData.ContainsIgnoreCase("_default")) ?? personGroups.FirstOrDefault())?.PersonGroupId;
            }
            catch
            {
            }
            finally
            {
                faceServiceInitialized = true;
            }
        }

        private async Task RaiseOnProgressAsync(Func<RecognitionPhase, Task> onProgress, RecognitionPhase phase)
        {
            var handler = onProgress;
            if (handler != null)
            {
                await handler.Invoke(phase);
            }
        }

        private async Task<CognitiveException> CreateExceptionAsync(string code, string message, string source, HttpStatusCode statusCode, Exception originalException, string language, Func<RecognitionPhase, Task> onProgress = null)
        {
            try
            {
                message = await TranslateAsync(message, language, onProgress);
            }
            catch
            {
            }

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
                await RaiseOnProgressAsync(onProgress, RecognitionPhase.Translating);

                translation = await translatorService.TranslateAsync(message, from: DefaultLanguge, to: language);
            }

            return translation;
        }

        public void Dispose()
        {
            translatorService.Dispose();
        }
    }
}