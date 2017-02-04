using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;

namespace See4Me.Engine.Services.TranslatorService
{
    /// <summary>
    /// The <strong>TranslatorServiceClient</strong> class provides methods to translate text to various supported languages.
    /// </summary>
    /// <remarks>
    /// <para>To use this library, you must register Microsoft Translator on https://portal.azure.com/#create/Microsoft.CognitiveServices/apitype/TextTranslation to obtain the Subscription key.
    /// </para>
    /// </remarks>
    public class TranslatorServiceClient : ITranslatorServiceClient, IDisposable
    {
        private const string BASE_URL = "http://api.microsofttranslator.com/v2/Http.svc/";
        private const string LANGUAGES_URI = "GetLanguagesForTranslate";
        private const string TRANSLATE_URI = "Translate?text={0}&to={1}&contentType=text/plain";
        private const string TRANSLATE_WITH_FROM_URI = "Translate?text={0}&from={1}&to={2}&contentType=text/plain";
        private const string DETECT_URI = "Detect?text={0}";
        private const string AUTHORIZATION_HEADER = "Authorization";

        private const string ARRAY_NAMESPACE = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

        private const int MAX_TEXT_LENGTH = 1000;
        private const int MAX_TEXT_LENGTH_FOR_AUTODETECTION = 100;

        private readonly AzureAuthToken authToken;
        private readonly HttpClient client;
        private string authorizationHeaderValue = string.Empty;

        #region Properties

        /// <summary>
        /// Gets or sets the Subscription key that is necessary to use <strong>Microsoft Translator Service</strong>.
        /// </summary>
        /// <value>The Subscription Key.</value>
        /// <remarks>
        /// <para>You must register Microsoft Translator on https://portal.azure.com/#create/Microsoft.CognitiveServices/apitype/TextTranslation to obtain the Subscription key needed to use the service.</para>
        /// </remarks>
        public string SubscriptionKey
        {
            get { return authToken.SubscriptionKey; }
            set { authToken.SubscriptionKey = value; }
        }

        /// <summary>
        /// Gets or sets the string representing the supported language code to translate the text to.
        /// </summary>
        /// <value>The string representing the supported language code to translate the text to. The code must be present in the list of codes returned from the method <see cref="GetLanguagesAsync"/>.</value>
        /// <seealso cref="GetLanguagesAsync"/>
        public string Language { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorServiceClient"/> class, using the current system language.
        /// </summary>
        /// <remarks>
        /// <para>You must register Microsoft Translator on https://portal.azure.com/#create/Microsoft.CognitiveServices/apitype/TextTranslation to obtain the Subscription key needed to use the service.</para>
        /// </remarks>
        /// <seealso cref="SubscriptionKey"/>
        /// <seealso cref="Language"/>
        public TranslatorServiceClient()
            : this(null, CultureInfo.CurrentCulture.Name.ToLower())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorServiceClient"/> class, using the specified Subscription key and the desired language.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key for the Microsoft Translator Service on Azure
        /// </param>
        /// <param name="language">A string representing the supported language code to translate the text to. The code must be present in the list of codes returned from the method <see cref="GetLanguagesAsync"/>. If a null value is provided, the current system language is used.
        /// </param>
        /// <remarks>
        /// <para>You must register Microsoft Translator on https://portal.azure.com to obtain the Subscription key needed to use the service.</para>
        /// </remarks>
        /// <seealso cref="SubscriptionKey"/>
        /// <seealso cref="Language"/>
        public TranslatorServiceClient(string subscriptionKey, string language = null)
        {
            authToken = new AzureAuthToken(subscriptionKey);
            client = new HttpClient { BaseAddress = new Uri(BASE_URL) };

            SubscriptionKey = subscriptionKey;
            Language = language ?? CultureInfo.CurrentCulture.Name.ToLower();
        }

        #region Get Languages

        /// <summary>
        /// Retrieves the languages available for speech synthesis.
        /// </summary>
        /// <returns>A string array containing the language codes supported for speech synthesis by <strong>Microsoft Translator Service</strong>.</returns>
        /// <exception cref="ArgumentNullException">The <see cref="SubscriptionKey"/> property hasn't been set.</exception>
        /// <exception cref="TranslatorServiceException">The provided <see cref="SubscriptionKey"/> isn't valid or has expired.</exception>
        /// <remarks><para>This method performs a non-blocking request for language codes.</para>
        /// <para>For more information, go to http://msdn.microsoft.com/en-us/library/ff512415.aspx.
        /// </para>
        /// </remarks>
        public async Task<IEnumerable<string>> GetLanguagesAsync()
        {
            // Check if it is necessary to obtain/update access token.
            await CheckUpdateTokenAsync().ConfigureAwait(false);

            var content = await client.GetStringAsync(LANGUAGES_URI).ConfigureAwait(false);

            XNamespace ns = ARRAY_NAMESPACE;
            var doc = XDocument.Parse(content);

            var languages = doc.Root.Elements(ns + "string").Select(s => s.Value);
            return languages;
        }

        #endregion

        #region Translate

        /// <summary>
        /// Translates a text string into the specified language.
        /// </summary>
        /// <returns>A string representing the translated text.</returns>
        /// <param name="text">A string representing the text to translate.</param>
        /// <param name="to">A string representing the language code to translate the text into. The code must be present in the list of codes returned from the <see cref="GetLanguagesAsync"/> method. If the parameter is set to <strong>null</strong>, the language specified in the <seealso cref="Language"/> property will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <term>The <see cref="SubscriptionKey"/> property hasn't been set.</term>
        /// <term>The <paramref name="text"/> parameter is <strong>null</strong> (<strong>Nothing</strong> in Visual Basic) or empty.</term>
        /// </list>
        /// </exception>        
        /// <exception cref="ArgumentException">The <paramref name="text"/> parameter is longer than 1000 characters.</exception>
        /// <exception cref="TranslatorServiceException">The provided <see cref="SubscriptionKey"/> isn't valid or has expired.</exception>
        /// <remarks><para>This method perform a non-blocking request for text translation.</para>
        /// <para>For more information, go to http://msdn.microsoft.com/en-us/library/ff512421.aspx.
        /// </para>
        /// </remarks>
        /// <seealso cref="Language"/>
        public Task<string> TranslateAsync(string text, string to = null) => TranslateAsync(text, null, to);

        /// <summary>
        /// Translates a text string into the specified language.
        /// </summary>
        /// <returns>A string representing the translated text.</returns>
        /// <param name="text">A string representing the text to translate.</param>
        /// <param name="from">A string representing the language code of the original text. The code must be present in the list of codes returned from the <see cref="GetLanguagesAsync"/> method. If the parameter is set to <strong>null</strong>, the language specified in the <seealso cref="Language"/> property will be used.</param>
        /// <param name="to">A string representing the language code to translate the text into. The code must be present in the list of codes returned from the <see cref="GetLanguagesAsync"/> method. If the parameter is set to <strong>null</strong>, the language specified in the <seealso cref="Language"/> property will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <term>The <see cref="SubscriptionKey"/> property hasn't been set.</term>
        /// <term>The <paramref name="text"/> parameter is <strong>null</strong> (<strong>Nothing</strong> in Visual Basic) or empty.</term>
        /// </list>
        /// </exception>        
        /// <exception cref="ArgumentException">The <paramref name="text"/> parameter is longer than 1000 characters.</exception>
        /// <exception cref="TranslatorServiceException">The provided <see cref="SubscriptionKey"/> isn't valid or has expired.</exception>
        /// <remarks><para>This method perform a non-blocking request for text translation.</para>
        /// <para>For more information, go to http://msdn.microsoft.com/en-us/library/ff512421.aspx.
        /// </para>
        /// </remarks>
        /// <seealso cref="Language"/>
        public async Task<string> TranslateAsync(string text, string from, string to)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            if (text.Length > MAX_TEXT_LENGTH)
                throw new ArgumentException($"{nameof(text)} parameter cannot be longer than {MAX_TEXT_LENGTH} characters");

            // Checks if it is necessary to obtain/update access token.
            await CheckUpdateTokenAsync().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(to))
                to = Language;

            string uri = null;
            if (string.IsNullOrWhiteSpace(from))
                uri = string.Format(TRANSLATE_URI, Uri.EscapeDataString(text), to);
            else
                uri = string.Format(TRANSLATE_WITH_FROM_URI, Uri.EscapeDataString(text), from, to);

            var content = await client.GetStringAsync(uri).ConfigureAwait(false);

            var doc = XDocument.Parse(content);
            var translatedText = doc.Root.Value;

            return translatedText;
        }

        #endregion

        #region Detect Language

        /// <summary>
        /// Detects the language of a text.
        /// </summary>
        /// <param name="text">A string represeting the text whose language must be detected.</param>
        /// <returns>A string containing a two-character Language code for the given text.</returns>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <term>The <see cref="SubscriptionKey"/> property hasn't been set.</term>
        /// <term>The <paramref name="text"/> parameter is <strong>null</strong> (<strong>Nothing</strong> in Visual Basic) or empty.</term>
        /// </list>
        /// </exception>        
        /// <exception cref="TranslatorServiceException">The provided <see cref="SubscriptionKey"/> isn't valid or has expired.</exception>
        /// <remarks><para>This method performs a non-blocking request for language detection.</para>
        /// <para>For more information, go to http://msdn.microsoft.com/en-us/library/ff512427.aspx.
        /// </para></remarks>
        /// <seealso cref="GetLanguagesAsync"/>
        /// <seealso cref="Language"/>
        public async Task<string> DetectLanguageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            text = text.Substring(0, Math.Min(text.Length, MAX_TEXT_LENGTH_FOR_AUTODETECTION));

            // Checks if it is necessary to obtain/update access token.
            await CheckUpdateTokenAsync().ConfigureAwait(false);

            var uri = string.Format(DETECT_URI, Uri.EscapeDataString(text));
            var content = await client.GetStringAsync(uri).ConfigureAwait(false);

            var doc = XDocument.Parse(content);
            var detectedLanguage = doc.Root.Value;

            return detectedLanguage;
        }

        #endregion

        private async Task CheckUpdateTokenAsync()
        {
            var token = await authToken.GetAccessTokenAsync().ConfigureAwait(false);
            if (token != authorizationHeaderValue)
            {
                // Updates the access token.
                authorizationHeaderValue = token;
                var headers = client.DefaultRequestHeaders;

                if (headers.Contains(AUTHORIZATION_HEADER))
                    headers.Remove(AUTHORIZATION_HEADER);

                headers.Add(AUTHORIZATION_HEADER, authorizationHeaderValue);
            }
        }

        /// <summary>
        /// Initializes the <see cref="TranslatorServiceClient"/> class by getting an access token for the service.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the initialize operation.</returns>
        /// <exception cref="ArgumentNullException">The <see cref="SubscriptionKey"/> property hasn't been set.</exception>
        /// <exception cref="TranslatorServiceException">The provided <see cref="SubscriptionKey"/> isn't valid or has expired.</exception>
        /// <remarks>Calling this method isn't mandatory, because the token is get/refreshed everytime is needed. However, it is called at startup, it can speed-up subsequest requests.</remarks>
        public Task InitializeAsync() => CheckUpdateTokenAsync();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            authToken.Dispose();
            client.Dispose();
        }
    }
}