using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTester
{
    internal class Connector : IConnector
    {
        private static readonly Dictionary<string, Type> Aliases = new Dictionary<Type, string>()
            {
                { typeof(byte), "byte" },
                { typeof(sbyte), "sbyte" },
                { typeof(short), "short" },
                { typeof(ushort), "ushort" },
                { typeof(int), "int" },
                { typeof(uint), "uint" },
                { typeof(long), "long" },
                { typeof(ulong), "ulong" },
                { typeof(float), "float" },
                { typeof(double), "double" },
                { typeof(decimal), "decimal" },
                { typeof(object), "object" },
                { typeof(bool), "bool" },
                { typeof(char), "char" },
                { typeof(string), "string" },
                { typeof(void), "void" }
            }.ToDictionary(x => x.Value, x => x.Key); // flemme d'inverser le tableau à la main

        private HubConnection? _con;

        public event Action<Exception?>? Closed;

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

        public void ListenTo(string methodName, IEnumerable<Parameter> parameters, Action<object?[]> callback)
        {
            if (_con == null)
            {
                throw new InvalidOperationException("Connection was not established");
            }

            Type[] parameterTypes = parameters
                            .Where(param => param.IsValid)
                            .Select(param =>
                            {
                                string typeName = param.Type!;
                                Aliases.TryGetValue(typeName, out Type? type);
                                type ??= Type.GetType(typeName);
                                return type!;
                            })
                            .ToArray();
            _con.On(methodName, parameterTypes, args =>
                {
                    callback.Invoke(args);
                    return Task.CompletedTask;
                });
        }
    }
}
