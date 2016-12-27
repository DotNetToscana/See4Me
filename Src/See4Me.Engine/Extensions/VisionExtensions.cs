using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using See4Me.Engine.Services.ServiceSettings;
using System.Net;
using System.Text.RegularExpressions;

namespace See4Me.Engine.Extensions
{
    internal static class VisionExtensions
    {
        private const string InvalidImageUrl = nameof(InvalidImageUrl);
        private const string InvalidImageFormat = nameof(InvalidImageFormat);
        private const string InvalidImageSize = nameof(InvalidImageSize);
        private const string NotSupportedImage = nameof(NotSupportedImage);
        private const string BadArgument = nameof(BadArgument);
        private const string FailedToProcess = nameof(FailedToProcess);
        private const string Timeout = nameof(Timeout);
        private const string InternalServerError = nameof(InternalServerError);

        private const string InvalidSubscriptionKeyMessage = "Access denied due to invalid subscription key. Make sure to provide a valid key for an active subscription.";

        private static Dictionary<HttpStatusCode, IEnumerable<string>> statusCodeErrorMapping;

        static VisionExtensions()
        {
            statusCodeErrorMapping = new Dictionary<HttpStatusCode, IEnumerable<string>>
            {
                [HttpStatusCode.BadRequest] = new List<string> { InvalidImageUrl, InvalidImageFormat, InvalidImageSize, NotSupportedImage },
                [HttpStatusCode.UnsupportedMediaType] = new List<string> { BadArgument },
                [HttpStatusCode.InternalServerError] = new List<string> { FailedToProcess, Timeout, InternalServerError }
            };
        }

        public static bool IsValid(this AnalysisResult result, out Caption rawDescription, out Caption filteredDescription, VisionSettings settings = null)
        {
            rawDescription = result.Description.Captions.FirstOrDefault();
            filteredDescription = null;

            // If there is no settings, all the descriptions are valid.
            if (settings == null)
            {
                filteredDescription = rawDescription;
                return true;
            }

            if (rawDescription?.Confidence >= settings.MinimumConfidence)
            {
                var text = rawDescription.Text;
                var replacedText = settings.DescriptionsToReplace.FirstOrDefault(d => d.Key.EqualsIgnoreCase(text)).Value;

                if (!string.IsNullOrWhiteSpace(replacedText))
                    text = replacedText;

                var textToRemove = settings.DescriptionsToRemove.FirstOrDefault(d => text.ContainsIgnoreCase(d));
                var filteredText = !string.IsNullOrWhiteSpace(textToRemove) ? text.ReplaceIgnoreCase(textToRemove, string.Empty).Trim() : text;

                if (!settings.InvalidDescriptions.Any(d => filteredText.ContainsIgnoreCase(d)))
                {
                    filteredDescription = new Caption
                    {
                        Text = filteredText,
                        Confidence = rawDescription.Confidence
                    };

                    return true;
                }
            }

            return false;
        }

        public static HttpStatusCode GetHttpStatusCode(this Microsoft.ProjectOxford.Vision.ClientException exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            var keyValue = statusCodeErrorMapping.FirstOrDefault(s => s.Value.Any(e => e == exception.Error.Code));
            if (keyValue.Value != null)
            {
                statusCode = keyValue.Key;
            }
            else
            {
                if (exception.Error.Message == InvalidSubscriptionKeyMessage)
                    statusCode = HttpStatusCode.Forbidden;
            }

            return statusCode;
        }
    }
}
