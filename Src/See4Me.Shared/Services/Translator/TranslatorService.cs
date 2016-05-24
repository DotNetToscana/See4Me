using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;

namespace See4Me.Services.Translator
{
    /// <summary>
    /// The <strong>TranslatorService</strong> class provides methods to translate text in various supported languages.
    /// </summary>
    /// <remarks>
    /// <para>To use this library, you need to go to <strong>Azure DataMarket</strong> at https://datamarket.azure.com/developer/applications and register your application. In this way, you'll obtain the <see cref="ClientId"/> and <see cref="ClientSecret"/> that are necessary to use <strong>Microsoft Translator Service</strong>.</para>
    /// <para>You also need to go to https://datamarket.azure.com/dataset/1899a118-d202-492c-aa16-ba21c33c06cb and subscribe the <strong>Microsoft Translator Service</strong>. There are many options, based on the amount of characters per month. The service is free up to 2 million characters per month.</para>
    /// </remarks>
    public sealed class TranslatorService : ITranslatorService
    {
        private const string BASE_URL = "http://api.microsofttranslator.com/v2/Http.svc/";
        private const string LANGUAGES_URI = "GetLanguagesForTranslate";
        private const string TRANSLATE_URI = "Translate?text={0}&to={1}&contentType=text/plain";
        private const string DETECT_URI = "Detect?text={0}";
        private const string AUTHORIZATION_HEADER = "Authorization";
        private const int MAX_TEXT_LENGTH = 1000;
        private const int MAX_TEXT_LENGTH_FOR_AUTODETECTION = 100;

        private DateTime tokenRequestTime;
        private int tokenValiditySeconds;
        private string headerValue;

        #region Properties

        /// <summary>
        /// Gets or sets the Application Client ID that is necessary to use <strong>Microsoft Translator Service</strong>.
        /// </summary>
        /// <value>The Application Client ID.</value>
        /// <remarks>
        /// <para>Go to <strong>Azure DataMarket</strong> at https://datamarket.azure.com/developer/applications to register your application and obtain a Client ID.</para>
        /// <para>You also need to go to https://datamarket.azure.com/dataset/1899a118-d202-492c-aa16-ba21c33c06cb and subscribe the <strong>Microsoft Translator Service</strong>. There are many options, based on the amount of characters per month. The service is free up to 2 million characters per month.</para>
        /// </remarks>        
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the Application Client Secret that is necessary to use <strong>Microsoft Translator Service</strong>.
        /// </summary>
        /// <remarks>
        /// <value>The Application Client Secret.</value>
        /// <para>Go to <strong>Azure DataMaket</strong> at https://datamarket.azure.com/developer/applications to register your application and obtain a Client Secret.</para>
        /// <para>You also need to go to https://datamarket.azure.com/dataset/1899a118-d202-492c-aa16-ba21c33c06cb and subscribe the <strong>Microsoft Translator Service</strong>. There are many options, based on the amount of characters per month. The service is free up to 2 million characters per month.</para>
        /// </remarks>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the string representing the supported language code to speak the text in.
        /// </summary>
        /// <value>The string representing the supported language code to speak the text in. The code must be present in the list of codes returned from the method <see cref="GetLanguagesAsync"/>.</value>
        /// <seealso cref="GetLanguagesAsync"/>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets a value that tells whether the TranslatorService is initialized.
        /// </summary>
        /// <value><strong>true</strong> if the TranslatorService is initialized, <strong>false</strong> otherwise.</value>
        public bool IsInitialized { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <strong>TranslatorService</strong> class, using the specified Client ID and Client Secret and the current system language.
        /// </summary>
        /// <param name="clientId">The Application Client ID.
        /// </param>
        /// <param name="clientSecret">The Application Client Secret.
        /// </param>
        /// <remarks><para>You must register your application on <strong>Azure DataMarket</strong> at https://datamarket.azure.com/developer/applications to obtain the Client ID and Client Secret needed to use the service.</para>
        /// <para>You also need to go to https://datamarket.azure.com/dataset/1899a118-d202-492c-aa16-ba21c33c06cb and subscribe the <strong>Microsoft Translator Service</strong>. There are many options, based on the amount of characters per month. The service is free up to 2 million characters per month.</para>
        /// </remarks>
        /// <seealso cref="ClientId"/>
        /// <seealso cref="ClientSecret"/>        
        /// <seealso cref="Language"/>
        public TranslatorService(string clientId, string clientSecret)
            : this(clientId, clientSecret, CultureInfo.CurrentCulture.Name.ToLower())
        { }

        /// <summary>
        /// Initializes a new instance of the <strong>TranslatorService</strong> class, using the specified Client ID and Client Secret and the desired language.
        /// </summary>
        /// <param name="clientId">The Application Client ID.
        /// </param>
        /// <param name="clientSecret">The Application Client Secret.
        /// </param>
        /// <param name="language">A string representing the supported language code to speak the text in. The code must be present in the list of codes returned from the method <see cref="GetLanguagesAsync"/>.</param>
        /// <remarks><para>You must register your application on <strong>Azure DataMarket</strong> at https://datamarket.azure.com/developer/applications to obtain the Client ID and Client Secret needed to use the service.</para>
        /// <para>You also need to go to https://datamarket.azure.com/dataset/1899a118-d202-492c-aa16-ba21c33c06cb and subscribe the <strong>Microsoft Translator Service</strong>. There are many options, based on the amount of characters per month. The service is free up to 2 million characters per month.</para>
        /// </remarks>
        /// <seealso cref="ClientId"/>
        /// <seealso cref="ClientSecret"/>        
        /// <seealso cref="Language"/>
        public TranslatorService(string clientId, string clientSecret, string language)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Language = language;
        }

        #region Get Languages

        /// <summary>
        /// Retrieves the languages available for speech synthesis.
        /// </summary>
        /// <returns>A string array containing the language codes supported for speech synthesis by <strong>Microsoft Translator Service</strong>.</returns>        
        /// <exception cref="ArgumentException">The <see cref="ClientId"/> or <see cref="ClientSecret"/> properties haven't been set.</exception>
        /// <remarks><para>This method performs a non-blocking request.</para>
        /// <para>For more information, go to http://msdn.microsoft.com/en-us/library/ff512415.aspx.
        /// </para>
        /// </remarks>    
        public async Task<IEnumerable<string>> GetLanguagesAsync()
        {
            // Check if it is necessary to obtain/update access token.
            await this.UpdateTokenAsync();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);
                client.DefaultRequestHeaders.Add(AUTHORIZATION_HEADER, headerValue);

                var content = await client.GetStringAsync(LANGUAGES_URI);

                XNamespace ns = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
                var doc = XDocument.Parse(content);

                var languages = doc.Root.Elements(ns + "string").Select(s => s.Value);
                return languages;
            }
        }

        #endregion

        #region Translate
        /// <summary>
        /// Translates a text string into the specified language.
        /// </summary>
        /// <returns>A string representing the translated text.</returns>
        /// <param name="text">A string representing the text to translate.</param>
        /// <param name="to">A string representing the language code to translate the text into. The code must be present in the list of codes returned from the <see cref="GetLanguagesAsync"/> method. If the parameter is set to <strong>null</strong>, the language specified in the <seealso cref="Language"/> property will be used.</param>
        /// <exception cref="ArgumentException">
        /// <list type="bullet">
        /// <term>The <see cref="ClientId"/> or <see cref="ClientSecret"/> properties haven't been set.</term>
        /// <term>The <paramref name="text"/> parameter is longer than 1000 characters.</term>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="text"/> parameter is <strong>null</strong> (<strong>Nothing</strong> in Visual Basic) or empty.</exception>
        /// <remarks><para>This method perform a non-blocking request for translation.</para>
        /// <para>For more information, go to http://msdn.microsoft.com/en-us/library/ff512421.aspx.
        /// </para>
        /// </remarks>
        /// <seealso cref="Language"/>  
        public async Task<string> TranslateAsync(string text, string to = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            if (text.Length > MAX_TEXT_LENGTH)
                throw new ArgumentException($"{nameof(text)} parameter cannot be longer than {MAX_TEXT_LENGTH} characters");

            // Checks if it is necessary to obtain/update access token.
            await this.UpdateTokenAsync();

            if (string.IsNullOrWhiteSpace(to))
                to = Language;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);
                client.DefaultRequestHeaders.Add(AUTHORIZATION_HEADER, headerValue);

                var uri = string.Format(TRANSLATE_URI, Uri.EscapeDataString(text), to);
                var content = await client.GetStringAsync(uri);

                var doc = XDocument.Parse(content);
                var translatedText = doc.Root.Value;

                return translatedText;
            }
        }

        #endregion

        #region Detect Language

        /// <summary>
        /// Detects the language of a text. 
        /// </summary>
        /// <param name="text">A string represeting the text whose language must be detected.</param>
        /// <returns>A string containing a two-character Language code for the given text.</returns>
        /// <exception cref="ArgumentException">
        /// <list type="bullet">
        /// <term>The <see cref="ClientId"/> or <see cref="ClientSecret"/> properties haven't been set.</term>
        /// <term>The <paramref name="text"/> parameter is longer than 1000 characters.</term>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="text"/> parameter is <strong>null</strong> (<strong>Nothing</strong> in Visual Basic) or empty.</exception>
        /// <remarks><para>This method perform a non-blocking request for language code.</para>
        /// <para>For more information, go to http://msdn.microsoft.com/en-us/library/ff512427.aspx.
        /// </para></remarks>
        /// <seealso cref="GetLanguagesAsync"/>
        /// <seealso cref="Language"/> 
        public async Task<string> DetectLanguageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            text = text.Substring(0, Math.Min(text.Length, MAX_TEXT_LENGTH_FOR_AUTODETECTION));

            // Check if it is necessary to obtain/update access token.
            await this.UpdateTokenAsync();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);
                client.DefaultRequestHeaders.Add(AUTHORIZATION_HEADER, headerValue);

                var uri = string.Format(DETECT_URI, Uri.EscapeDataString(text));
                var content = await client.GetStringAsync(uri);

                var doc = XDocument.Parse(content);
                var detectedLanguage = doc.Root.Value;

                return detectedLanguage;
            }
        }

        #endregion

        private async Task UpdateTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
                throw new ArgumentException("Invalid Client ID. Go to Azure Marketplace and sign up for Microsoft Translator: https://datamarket.azure.com/developer/applications");

            if (string.IsNullOrWhiteSpace(ClientSecret))
                throw new ArgumentException("Invalid Client Secret. Go to Azure Marketplace and sign up for Microsoft Translator: https://datamarket.azure.com/developer/applications");

            if ((DateTime.Now - tokenRequestTime).TotalSeconds > tokenValiditySeconds)
            {
                // Token has expired. Make a request for a new one.
                tokenRequestTime = DateTime.Now;
                var admAuth = new AdmAuthentication(ClientId, ClientSecret);

                try
                {
                    var admToken = await admAuth.GetAccessTokenAsync();

                    tokenValiditySeconds = int.Parse(admToken.ExpiresIn);
                    headerValue = "Bearer " + admToken.AccessToken;
                    IsInitialized = true;
                }
                catch
                { }
            }
        }

        public Task InitializeAsync() => this.UpdateTokenAsync();
    }
}
