using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTester.Components
{
    public interface ITypesLoader
    {
        IEnumerable<string> TypeNames { get; }

        IEnumerable<string> LoadedDlls { get; }

        IEnumerable<string> LoadDlls(string[] fileNames);

        Type GetType(string typeName);

        void ClearTypes();

    }
}
