using GalaSoft.MvvmLight;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public class NetworkService : INetworkService, IDisposable
    {
        private const string CONNECTION_TEST_URL = "http://www.google.com";
        private const int CONNECTION_TEST_TIMEOUT_SECONDS = 3;

        public bool IsConnected { get; private set; }

        public event EventHandler ConnectivityChanged;

        public NetworkService()
        {
            IsConnected = CrossConnectivity.Current.IsConnected;
            CrossConnectivity.Current.ConnectivityChanged += OnConnectivityChanged;
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsConnected = e.IsConnected;
            ConnectivityChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task<bool> GetIsInternetAvailableAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(CONNECTION_TEST_TIMEOUT_SECONDS);
                    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, CONNECTION_TEST_URL));
                    response.EnsureSuccessStatusCode();

                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public void Dispose()
        {
            CrossConnectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
        }
    }
}
