using System.ComponentModel;
using System.Runtime.CompilerServices;
using SignalRTester.Dto;

namespace SignalRTester.Models
{
    public class Parameter : INotifyPropertyChanged
    {

        private string? _type;
        private string? _name;

        public string? Type
        {
            get => _type; set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public string? Name
        {
            get => _name; set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool IsValid => !string.IsNullOrEmpty(_type) && !string.IsNullOrEmpty(_name);

        public event PropertyChangedEventHandler? PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Parameter() { }

        public Parameter(ParameterDto dto)
        {
            Name = dto.Name;
            Type = dto.Type;
        }

        public ParameterDto GetDto() => new()
        {
            Name = Name,
            Type = Type
        };

    }
}
