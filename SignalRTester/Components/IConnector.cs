using Microsoft.AspNetCore.SignalR.Client;
using SignalRTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRTester.Components
{
    public interface IConnector
    {
        event Action<Exception?> Closed;

        Task<string> ConnectAsync(string url, IEnumerable<Header> headers);

        void ListenTo(Method method, Action<object?[]> callback);

        Task DisconnectAsync();
    }
}
