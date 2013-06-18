using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Ids
{
    public class Id
    {
        private readonly IIdService idService;

        public Id(IIdService ids) { idService = ids; }

        public string GetSymbol(SymbolType type)
        {
            return idService.GetSymbol(this, type);
        }

        public IEnumerable<SymbolType> AllTypes
        {
            get { return idService.AllTypes; }
        }

        public override string ToString()
        {
            return AllTypes.Select(type => GetSymbol(type)).Join(",");
        }
    }
}
