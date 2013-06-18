using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Plugins;

namespace Toolkit.Ids
{
    public interface IIdService
    {
        IEnumerable<Id> AllIds { get; }
        IEnumerable<SymbolType> AllTypes { get; }

        string GetSymbol(Id id, SymbolType type);
        Id GetId(string symbol, SymbolType type);
    }

    // get from type + maps and can configure map name in the services
    public class BaseIdService : IIdService
    {
        private Dictionary<SymbolType, Dictionary<Id, string>> txmaps = new Dictionary<SymbolType, Dictionary<Id, string>>();
        private Dictionary<SymbolType, Dictionary<string, Id>> rxmaps = new Dictionary<SymbolType, Dictionary<string, Id>>();

        public IEnumerable<Id> AllIds
        {
            get { return txmaps.Values.First().Keys; }
        }

        public IEnumerable<SymbolType> AllTypes
        {
            get { return txmaps.Keys; }
        }

        public string GetSymbol(Id id, SymbolType type)
        {
            return txmaps[type][id];
        }

        public Id GetId(string symbol, SymbolType type)
        {
            return rxmaps[type][symbol];
        }

        public IEnumerable<string> ToCsv()
        {
            var header = txmaps.Keys.Join(",");
            var data = AllIds.Select(id => AllTypes.Select(type => GetSymbol(id, type)).Join(","));
            return new string[] { header }.Concat(data);
        }

        protected void Add(Id id, string[] symbols, SymbolType[] types)
        {
            if (symbols.Length != types.Length)
                throw new ArgumentException("symbols.Length != types.Length");

            for (int i = 0; i < symbols.Length; i++)
                Add(id, symbols[i].Trim(), types[i]);
        }

        private void Add(Id id, string symbol, SymbolType type)
        {
            Dictionary<Id, string> txmap = null;
            if (!txmaps.TryGetValue(type, out txmap))
            {
                txmap = new Dictionary<Id, string>();
                txmaps.Add(type, txmap);
            }
            txmap[id] = symbol;

            Dictionary<string, Id> rxmap = null;
            if (!rxmaps.TryGetValue(type, out rxmap))
            {
                rxmap = new Dictionary<string, Id>();
                rxmaps.Add(type, rxmap);
            }
            rxmap[symbol] = id;
        }
    }
}
