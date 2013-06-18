using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Plugins;

namespace Toolkit.Configuration
{
    public class InstrumentsConfiguration : IPlugin
    {
        private string[] headers;
        private Dictionary<string, InstrumentConfiguration> data;

        public void Initialize(Core.IEnvironment env, IConfiguration cfg)
        {
            Read(cfg.getString("source"));
        }

        public void Read(string filename)
        {
            Tuple<string[], IEnumerable<string[]>> csv = IO.Utils.ReadCSV(filename);

            this.headers = csv.Item1;
            this.data = csv.Item2.ToDictionary(d => d[0], d => new InstrumentConfiguration(headers, d.Skip(1).ToArray()));
        }

        public IEnumerable<string> Symbols
        {
            get { return data.Keys; }
        }

        public int SymbolCount
        {
            get { return data.Keys.Count; }
        }

        public IEnumerable<string> Keys
        {
            get { return headers; }
        }

        public bool HasKey(string key)
        {
            return Array.Exists(headers, s => s == key);
        }

        public InstrumentConfiguration getInstrument(string symbol)
        {
            return data[symbol];
        }

    }

    public class InstrumentConfiguration : BaseConfiguration
    {
        public InstrumentConfiguration(string[] headers, string[] data) : base(Parse(headers, data)) { }

        private static IEnumerable<string[]> Parse(string[] headers, string[] data)
        {
            return from header in headers
                   from line in data
                   select new string[] { header, line };
        }
    }
}
