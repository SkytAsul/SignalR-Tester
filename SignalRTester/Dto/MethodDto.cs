using SignalRTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTester.Dto
{
    public class MethodDto
    {
        public string? MethodName { get; set; }

        public IEnumerable<ParameterDto>? Parameters { get; set; }
    }
}
