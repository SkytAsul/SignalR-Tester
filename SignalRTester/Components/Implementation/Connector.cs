using Microsoft.AspNetCore.SignalR.Client;
using SignalRTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTester.Components.Implementation
{
    internal class Connector : IConnector
    {
        private readonly ITypesLoader _typesLoader;
        private HubConnection? _con;

        public event Action<Exception?>? Closed;

        public Connector(ITypesLoader typesLoader)
        {
            _typesLoader = typesLoader;
        }

        public async Task<string> ConnectAsync(string url, IEnumerable<Header> headers)
        {
            if (_con != null)
            {
                throw new InvalidOperationException("Connection already established");
            }

            _con = new HubConnectionBuilder()
                .WithUrl(url, o => o.Headers = headers
                        .Where(header => header.IsValid)
                        .ToDictionary(header => header.Key!, header => header.Value!))
                .Build();

            _con.Closed += Connection_Closed;

            await _con.StartAsync();

            return _con.ConnectionId ?? "unknown";
        }

        private Task Connection_Closed(Exception? arg)
        {
            _con = null;
            Closed?.Invoke(arg);
            return Task.CompletedTask;
        }

        public async Task DisconnectAsync()
        {
            if (_con == null)
            {
                throw new InvalidOperationException("Connection was not established");
            }

            var tmp = _con;
            _con = null;
            await tmp.StopAsync();
        }

        public void ListenTo(Method method, Action<object?[]> callback)
        {
            if (_con == null)
            {
                throw new InvalidOperationException("Connection was not established");
            }

            Type[] parameterTypes = method.Parameters
                            .Where(param => param.IsValid)
                            .Select(param => _typesLoader.GetType(param.Type!))
                            .ToArray();
            _con.On(method.MethodName!, parameterTypes, args =>
                {
                    callback.Invoke(args);
                    return Task.CompletedTask;
                });
        }
    }
}
