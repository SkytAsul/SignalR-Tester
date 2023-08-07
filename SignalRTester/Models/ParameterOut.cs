using SignalRTester.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTester.Models
{
    public class ParameterOut : Parameter
    {
        private string? _value;

        public string? Value
        {
            get => _value; set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public ParameterOut() { }

        public ParameterOut(ParameterOutDto dto) : base(dto)
        {
            Value = dto.Value;
        }

        public new ParameterOutDto GetDto() => new()
        {
            Name = Name,
            Type = Type,
            Value = Value,
        };
    }
}
