using SignalRTester.Dto;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace SignalRTester.Models
{
    public abstract class MethodBase<ParameterType> : INotifyPropertyChanged where ParameterType : Parameter
    {
        private const string DEFAULT_METHOD_NAME = "<new method>";

        private string? _methodName = DEFAULT_METHOD_NAME;

        public string? MethodName
        {
            get => _methodName; set
            {
                bool wasValid = IsValid;

                _methodName = value;
                OnPropertyChanged();

                if (wasValid != IsValid)
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public ObservableCollection<ParameterType> Parameters { get; } = new();

        public bool IsValid => MethodName != DEFAULT_METHOD_NAME;

        public event PropertyChangedEventHandler? PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MethodBase() { }

        public MethodBase(string methodName, IEnumerable<ParameterType> parameters)
        {
            MethodName = methodName;
            Parameters = new(parameters);
        }

    }
}
