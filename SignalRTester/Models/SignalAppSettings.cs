using SignalRTester.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTester.Models
{
    public class SignalAppSettings
    {
        public string? Url { get; set; }

        public IEnumerable<Header>? Headers { get; set; }

        public IEnumerable<MethodInDto>? IncomingMethods { get; set; }

        public IEnumerable<MethodOutDto>? OutgoingMethods { get; set; }

        public IEnumerable<string>? LoadedDlls { get; set; }
    }
}
