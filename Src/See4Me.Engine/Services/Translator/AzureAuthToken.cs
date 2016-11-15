using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace See4Me.Engine.Services.Translator
{
    /// <summary>
    /// Client to call Cognitive Services Azure Auth Token service in order to get an access token.
    /// </summary>
    internal class AzureAuthToken
    {
        /// URL of the token service
        private static readonly Uri ServiceUrl = new Uri("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");

        /// Name of header used to pass the subscription key to the token service
        private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

        /// After obtaining a valid token, this class will cache it for this duration.
        /// Use a duration of 5 minutes, which is less than the actual token lifetime of 10 minutes.
        private static readonly TimeSpan TokenCacheDuration = new TimeSpan(0, 5, 0);

        /// Cache the value of the last valid token obtained from the token service.
        private string storedTokenValue = string.Empty;

        /// When the last valid token was obtained.
        private DateTime storedTokenTime = DateTime.MinValue;

        private string subscriptionKey;
        /// Gets the subscription key.
        public string SubscriptionKey
        {
            get { return subscriptionKey; }
            set
            {
                if (subscriptionKey != value)
                {
                    // If the subscription key is changed, the token is no longer valid.
                    subscriptionKey = value;
                    storedTokenValue = string.Empty;
                }
            }
        }

        private readonly HttpClient client;

        /// <summary>
        /// Creates a client to obtain an access token.
        /// </summary>
        /// <param name="key">Subscription key to use to get an authentication token.</param>
        public AzureAuthToken(string key)
        {
            SubscriptionKey = key;
            client = new HttpClient();
        }

        /// <summary>
        /// Gets a token for the specified subscription.
        /// </summary>
        /// <returns>The encoded JWT token prefixed with the string "Bearer ".</returns>
        /// <remarks>
        /// This method uses a cache to limit the number of request to the token service.
        /// A fresh token can be re-used during its lifetime of 10 minutes. After a successful
        /// request to the token service, this method caches the access token. Subsequent
        /// invocations of the method return the cached token for the next 5 minutes. After
        /// 5 minutes, a new token is fetched from the token service and the cache is updated.
        /// </remarks>
        public async Task<string> GetAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(SubscriptionKey))
                throw new ArgumentNullException(nameof(SubscriptionKey), "A subscription key is required");

            // Re-use the cached token if there is one.
            if ((DateTime.Now - storedTokenTime) < TokenCacheDuration && !string.IsNullOrWhiteSpace(storedTokenValue))
                return storedTokenValue;

            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = ServiceUrl;
                request.Content = new StringContent(string.Empty);
                request.Headers.TryAddWithoutValidation(OcpApimSubscriptionKeyHeader, this.SubscriptionKey);

                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var token = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                storedTokenTime = DateTime.Now;
                storedTokenValue = $"Bearer {token}";

                return storedTokenValue;
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
