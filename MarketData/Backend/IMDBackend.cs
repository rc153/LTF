using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.IO;

namespace Toolkit.MarketData.Backend
{
    public interface IMDBackend
    {
        CompactBinaryReader getReader(string symbol);
    }
}
