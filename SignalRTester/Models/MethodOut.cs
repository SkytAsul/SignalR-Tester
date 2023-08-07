using SignalRTester.Dto;
using System.Linq;

namespace SignalRTester.Models
{
    public class MethodOut : MethodBase<ParameterOut>
    {
        public MethodOut() { }

        public MethodOut(MethodOutDto dto) : base(dto.MethodName, dto.Parameters.Select(parameter => new ParameterOut(parameter)))
        {
        }

        public MethodOutDto GetDto() => new()
        {
            MethodName = MethodName,
            Parameters = Parameters.Select(param => param.GetDto()).ToList()
        };
    }
}
