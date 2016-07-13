using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public interface INetworkService
    {
        bool IsConnected { get; }

        event EventHandler ConnectivityChanged;

        Task<bool> IsInternetAvailableAsync();
    }
}
