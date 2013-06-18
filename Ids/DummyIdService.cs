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
    public class DummyIdService : BaseIdService, IPlugin
    {
        public void Initialize(IEnvironment env, IConfiguration cfg)
        {
        }

        public Id CreateId(string[] symbols, SymbolType[] types)
        {
            Id id = new Id(this);
            Add(id, symbols, types);
            return id;
        }
    }
}
