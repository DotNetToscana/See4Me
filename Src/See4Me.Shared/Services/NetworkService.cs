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
        private readonly IConnectivity connectionManager;

        private const string CONNECTION_TEST_URL = "https://www.google.com";
        private const int CONNECTION_TEST_TIMEOUT_MILLISECONDS = 5000;

        public bool IsConnected { get; private set; }

        public event EventHandler ConnectivityChanged;

        public NetworkService()
        {
            connectionManager = CrossConnectivity.Current;

            IsConnected = connectionManager.IsConnected;
            connectionManager.ConnectivityChanged += OnConnectivityChanged;
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsConnected = e.IsConnected;
            ConnectivityChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task<bool> IsInternetAvailableAsync()
        {
            try
            {
                if (IsConnected)
                {
                    var isReachable = await connectionManager.IsRemoteReachable(CONNECTION_TEST_URL, msTimeout: CONNECTION_TEST_TIMEOUT_MILLISECONDS);
                    return isReachable;
                }
            }
            catch { }

            return false;
        }

        public void Dispose()
        {
            connectionManager.ConnectivityChanged -= OnConnectivityChanged;
        }
    }
}
