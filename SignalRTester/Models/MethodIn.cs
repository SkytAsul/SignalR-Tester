using SignalRTester.Dto;
using System.Linq;

namespace SignalRTester.Models
{
    public class MethodIn : MethodBase<Parameter>
    {
        public MethodIn() { }

        public MethodIn(MethodInDto dto) : base(dto.MethodName, dto.Parameters.Select(parameter => new Parameter(parameter)))
        {
        }

        public MethodInDto GetDto() => new()
        {
            MethodName = MethodName,
            Parameters = Parameters.Select(param => param.GetDto()).ToList()
        };
    }
}
