using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Ids;
using Toolkit.MarketData;
using Toolkit.Position;
using Toolkit.TickSize;

namespace Toolkit.Book
{
    // todo get the PMS infra to update this directly
    // Position = net quantity in the book
    // Ntnl = cash flow
    // Nominal = Position * current price * mult = nominal at risk
    public partial class Instrument : IPositionModel
    {
        public uint TradedQty
        {
            get { throw new NotImplementedException(); }
        }

        public double TradedNtnl
        {
            get { throw new NotImplementedException(); }
        }

        public uint Position
        {
            get { throw new NotImplementedException(); }
        }

        // todo we can cache that
        public double Nominal
        {
            get { return Position * MidPrice.ToDouble() * multiplier; }
        }
    }
}
