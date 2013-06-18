using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Position
{
    public interface IPositionModel
    {
        uint TradedQty { get; }
        double TradedNtnl { get; }      // FPD?

        uint Position { get; }
    }
}
