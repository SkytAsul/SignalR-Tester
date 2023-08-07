using SignalRTester.Models;
using SignalRTester.Properties;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SignalRTester.Components;
using SignalRTester.Components.Implementation;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace SignalRTester.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string? _url = Settings.Default.SavedUrl;
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

        public bool IsConnected
        {
            get => _isConnected; set
            {
                _isConnected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEditable));
            }
        }

        public bool IsEditable => !IsConnected;

        public ObservableCollection<Header> Headers { get; } = new();

        public ObservableCollection<MethodIn> IncomingMethods { get; } = new();
        
        public ObservableCollection<MethodOut> OutgoingMethods { get; } = new();

        public string Output => _outputBuilder.ToString();

        public ITypesLoader TypesLoader { get; } = new TypesLoader();

        private readonly IConnector _connector;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindowViewModel()
        {
            _connector = new Connector(TypesLoader);
            _connector.Closed += Connector_Closed;
            AddMethodInTab();
            AddMethodOutTab();
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

                foreach (MethodIn method in IncomingMethods.Where(m => m.IsValid))
                {
                    _connector.ListenTo(method, args =>
                    {
                        LogOutput("Method called! Received:");
                        for (int i = 0; i < method.Parameters.Count; i++)
                        {
                            Parameter parameter = method.Parameters[i];
                            var options = new JsonSerializerOptions()
                            {
                                WriteIndented = true
                            };
                            LogOutput($"{parameter.Type} {parameter.Name} = {JsonSerializer.Serialize(args[i], options)}");
                        }
                    });
                }
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
            Settings.Default.Save();
        }

        private void AddMethodInTab()
        {
            var method = new MethodIn();
            method.PropertyChanged += MethodIn_PropertyChanged;
            IncomingMethods.Add(method);
        }

        private void MethodIn_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MethodIn.IsValid))
            {
                if (AreAllMethodsInValid())
                {
                    AddMethodInTab();
                }
            }
        }

        private bool AreAllMethodsInValid() => IncomingMethods.All(m => m.IsValid);

        public bool CanRemoveIn(MethodIn method) => method.IsValid || IncomingMethods.Count(m => m.IsValid) >= 2;

        private void AddMethodOutTab()
        {
            var method = new MethodOut();
            method.PropertyChanged += MethodOut_PropertyChanged;
            OutgoingMethods.Add(method);
        }

        private void MethodOut_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MethodOut.IsValid))
            {
                if (AreAllMethodsOutValid())
                {
                    AddMethodOutTab();
                }
            }
        }

        private bool AreAllMethodsOutValid() => OutgoingMethods.All(m => m.IsValid);

        public bool CanRemoveOut(MethodOut method) => method.IsValid || OutgoingMethods.Count(m => m.IsValid) >= 2;

        public void LoadDlls(string[] fileNames)
        {
            LogOutput($"Loading {fileNames.Length} DLLs...");
            var loadedTypes = TypesLoader.LoadDlls(fileNames);
            LogOutput($"Loaded {loadedTypes.Count()} types from {string.Join(", ", fileNames)}");
        }

        public async Task SaveTo(string filename)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var serialized = JsonSerializer.Serialize(ToAppSettings(), options);
            await File.WriteAllTextAsync(filename, serialized);
        }

        public async Task Load(string filename)
        {
            var serialized = await File.ReadAllTextAsync(filename);
            var settings = JsonSerializer.Deserialize<SignalAppSettings>(serialized);

            Url = settings.Url;

            TypesLoader.ClearTypes();
            LoadDlls(settings.LoadedDlls.ToArray());

            IncomingMethods.Clear();
            foreach(var method in settings.IncomingMethods)
            {
                IncomingMethods.Add(new MethodIn(method));
            }
            AddMethodInTab();

            OutgoingMethods.Clear();
            foreach(var method in settings.OutgoingMethods)
            {
                OutgoingMethods.Add(new MethodOut(method));
            }
            AddMethodOutTab();

            Headers.Clear();
            foreach(var header in settings.Headers)
            {
                Headers.Add(header);
            }
        }

        public SignalAppSettings ToAppSettings()
        {
            return new SignalAppSettings()
            {
                Url = Url,
                Headers = Headers.Where(header => header.IsValid),
                IncomingMethods = IncomingMethods.Where(method => method.IsValid).Select(method => method.GetDto()).ToList(),
                OutgoingMethods = OutgoingMethods.Where(method => method.IsValid).Select(method => method.GetDto()).ToList(),
                LoadedDlls = TypesLoader.LoadedDlls
            };
        }

        public void SendMethod(MethodOut method)
        {
            Task.Run(async () =>
            {
                LogOutput($"Sending method {method.MethodName}...");
                try
                {
                    await _connector.SendMethodAsync(method);
                    LogOutput($"Method sent!");
                }
                catch (Exception ex)
                {
                    LogOutput($"Failed to send method. Exception: \n{ex}");
                }
            });
        }
    }
}
