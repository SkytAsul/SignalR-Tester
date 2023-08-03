using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRTester
{
    public interface IConnector
    {
        event Action<Exception?> Closed;

        Task<string> ConnectAsync(string url, IEnumerable<Header> headers);

        void ListenTo(string methodName, IEnumerable<Parameter> parameters, Action<object?[]> callback);

        Task DisconnectAsync();
    }
}
