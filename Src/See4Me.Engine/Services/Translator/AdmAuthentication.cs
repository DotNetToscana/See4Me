using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace See4Me.Engine.Services.Translator
{
    internal class AdmAccessToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }

    internal class AdmAuthentication
    {
        private const string DATAMARKET_ACCESS_URI = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private const string AzureScope = "http://api.microsofttranslator.com";

        private string clientId;
        private string clientSecret;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        public async Task<AdmAccessToken> GetAccessTokenAsync()
        {
            // get Azure Data Market token
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(DATAMARKET_ACCESS_URI, new FormUrlEncodedContent(
                    new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("client_secret", clientSecret),
                        new KeyValuePair<string, string>("scope", AzureScope),
                    })).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var admAccessToken = JsonConvert.DeserializeObject<AdmAccessToken>(json);
                return admAccessToken;
            }
        }
    }
}
