using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Ids
{
    public struct SymbolType
    {
        private static readonly object sync = new object();
        private static ImmutableDictionary<string, SymbolType> all = ImmutableDictionary<string, SymbolType>.Empty;
        private string Name;

        public static readonly SymbolType OMS = new SymbolType("OMS");

        private SymbolType(string name)
        {
            this.Name = name;
        }

        public static SymbolType Parse(string name)
        {
            SymbolType result;
            if (!all.TryGetValue(name, out result))
            {
                result = new SymbolType(name);
                lock (sync) all = all.Add(name, result);
            }
            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
