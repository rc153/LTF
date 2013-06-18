using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Plugins;

namespace Toolkit.Ids
{
    public class CsvIdService : BaseIdService, IPlugin
    {
        public void Initialize(IEnvironment env, IConfiguration cfg)
        {
            string path = cfg.getString("path");
            var file = File.ReadLines(path);
            SymbolType[] types = file.First().Split(',').Select(str => SymbolType.Parse(str)).ToArray();
            foreach (string[] symbols in file.Skip(1).Select(line => line.Split(',')))
                Add(new Id(this), symbols, types);
        }
    }
}
