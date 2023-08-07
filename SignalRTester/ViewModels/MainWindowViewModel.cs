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

        public ObservableCollection<Header> Headers { get; } = new ObservableCollection<Header>();

        public ObservableCollection<Method> Methods { get; } = new ObservableCollection<Method>();

        public string Output => _outputBuilder.ToString();

        public ITypesLoader TypesLoader { get; } = new TypesLoader();

        private readonly IConnector _connector;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindowViewModel()
        {
            _connector = new Connector(TypesLoader);
            _connector.Closed += Connector_Closed;
            AddMethodTab();
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

                foreach (Method method in Methods.Where(m => m.IsValid))
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

        private void AddMethodTab()
        {
            var method = new Method();
            method.PropertyChanged += Method_PropertyChanged;
            Methods.Add(method);
        }

        private void Method_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Method.IsValid))
            {
                if (AreAllMethodsValid())
                {
                    AddMethodTab();
                }
            }
        }

        private bool AreAllMethodsValid() => Methods.All(m => m.IsValid);

        public bool CanRemove(Method method) => method.IsValid || Methods.Count(m => m.IsValid) >= 2;

        public void LoadDlls(string[] fileNames)
        {
            LogOutput($"Loading {fileNames.Length} DLLs...");
            var loadedTypes = TypesLoader.LoadDlls(fileNames);
            LogOutput($"Loaded {loadedTypes.Count()} types:\n{string.Join("\n", loadedTypes)}");
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

            Methods.Clear();
            foreach(var method in settings.Methods)
            {
                Methods.Add(new Method(method));
            }
            AddMethodTab();

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
                Methods = Methods.Select(method => method.GetDto()).ToList(),
                LoadedDlls = TypesLoader.LoadedDlls
            };
        }
    }
}
