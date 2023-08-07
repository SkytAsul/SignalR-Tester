using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SignalRTester.Models
{
    public class Header : INotifyPropertyChanged
    {
        private string? _key;
        private string? _value;

        public string? Key
        {
            get => _key; set
            {
                _key = value;
                OnPropertyChanged();
            }
        }

        public string? Value
        {
            get => _value; set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrEmpty(_key) && !string.IsNullOrEmpty(_value);

        public event PropertyChangedEventHandler? PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
