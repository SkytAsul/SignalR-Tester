using SignalRTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTester.Dto
{
    public abstract class MethodBaseDto<ParameterType> where ParameterType : ParameterDto
    {
        public string? MethodName { get; set; }

        public IEnumerable<ParameterType>? Parameters { get; set; }
    }
}
