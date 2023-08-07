using SignalRTester.Dto;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace SignalRTester.Models
{
    public class Method : INotifyPropertyChanged
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

        public ObservableCollection<Parameter> Parameters { get; } = new ObservableCollection<Parameter>();

        public bool IsValid => MethodName != DEFAULT_METHOD_NAME;

        public event PropertyChangedEventHandler? PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Method() { }

        public Method(MethodDto dto)
        {
            MethodName = dto.MethodName;
            Parameters = new(dto.Parameters.Select(parameter => new Parameter(parameter)));
        }

        public MethodDto GetDto() => new()
        {
            MethodName = MethodName,
            Parameters = Parameters.Select(param => param.GetDto()).ToList()
        };

    }
}
