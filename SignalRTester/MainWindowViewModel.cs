using Microsoft.AspNetCore.Connections.Features;
using SignalRTester.Properties;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTester
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string? _url = Settings.Default.SavedUrl;
        private string? _methodName = Settings.Default.SavedMethodName;
        private bool _isConnected = false;
        private readonly StringBuilder _outputBuilder = new StringBuilder();

        public string? Url
        {
            get => _url; set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        public string? MethodName
        {
            get => _methodName; set
            {
                _methodName = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnected
        {
            get => _isConnected; set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Header> Headers { get; } = new ObservableCollection<Header>();

        public ObservableCollection<Parameter> Parameters { get; } = new ObservableCollection<Parameter>();

        public string Output => _outputBuilder.ToString();

        private readonly IConnector _connector = new Connector();

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindowViewModel()
        {
            _connector.Closed += Connector_Closed;
        }

        private void Connector_Closed(Exception? obj)
        {
            IsConnected = false;
            if (obj == null)
            {
                LogOutput("Connection closed");
            }
            else
            {
                LogOutput($"Connection closed with exception:\n{obj}");
            }
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void LogOutput(string str)
        {
            _outputBuilder.AppendLine(str);
            OnPropertyChanged(nameof(Output));
        }

        public void ClearOutput()
        {
            _outputBuilder.Clear();
            OnPropertyChanged(nameof(Output));
        }

        public async Task ConnectAsync()
        {
            LogOutput($"Trying to connect to {Url}...");

            try
            {
                string connectionId = await _connector.ConnectAsync(Url!, Headers);
                LogOutput($"Connected with connection id {connectionId}!");
                IsConnected = true;

                _connector.ListenTo(MethodName!, Parameters, args =>
                {
                    LogOutput("Method called! Received:");
                    for (int i = 0; i < Parameters.Count; i++)
                    {
                        Parameter parameter = Parameters[i];
                        LogOutput($"{parameter.Type} {parameter.Name} = {args[i]}");
                    }
                });
            }
            catch (Exception ex)
            {
                LogOutput($"Failed to connect. Exception: \n{ex}");
                await DisconnectAsync();
            }
        }

        public async Task DisconnectAsync()
        {
            await _connector.DisconnectAsync();
        }

        internal void SaveSettings()
        {
            Settings.Default.SavedUrl = Url;
            Settings.Default.SavedMethodName = MethodName;
            Settings.Default.Save();
        }
    }
}
