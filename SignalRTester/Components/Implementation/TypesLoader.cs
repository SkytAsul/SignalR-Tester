using SignalRTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SignalRTester.Components.Implementation
{
    public class TypesLoader : ITypesLoader
    {
        private static readonly Dictionary<string, Type> Aliases = new Dictionary<Type, string>()
            {
                { typeof(byte), "byte" },
                { typeof(sbyte), "sbyte" },
                { typeof(short), "short" },
                { typeof(ushort), "ushort" },
                { typeof(int), "int" },
                { typeof(uint), "uint" },
                { typeof(long), "long" },
                { typeof(ulong), "ulong" },
                { typeof(float), "float" },
                { typeof(double), "double" },
                { typeof(decimal), "decimal" },
                { typeof(object), "object" },
                { typeof(bool), "bool" },
                { typeof(char), "char" },
                { typeof(string), "string" },
                { typeof(void), "void" }
            }.ToDictionary(x => x.Value, x => x.Key); // flemme d'inverser le tableau à la main

        private Dictionary<string, Type> _types = new(Aliases);
        private readonly List<string> _loadedDlls = new();

        public IEnumerable<string> TypeNames => _types.Keys;

        public IEnumerable<string> LoadedDlls => _loadedDlls;

        public IEnumerable<string> LoadDlls(string[] fileNames)
        {
            var loaded = new List<string>();
            foreach (var fileName in fileNames)
            {
                var dll = Assembly.LoadFile(fileName);

                foreach(Type type in dll.GetExportedTypes())
                {
                    _types.Add(type.FullName!, type);
                    loaded.Add(type.FullName!);
                }

                _loadedDlls.Add(fileName);
            }
            return loaded;
        }

        public Type GetType(string typeName)
        {
            _types.TryGetValue(typeName, out Type? type);
            type ??= Type.GetType(typeName);

            if (type == null)
            {
                throw new TypeAccessException(typeName);
            }

            return type;
        }

        public void ClearTypes()
        {
            _types = new Dictionary<string, Type>(Aliases);
            _loadedDlls.Clear();
        }
    }
}
