using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.MarketData.SimpleQuote
{
    // to see, public class can inherit from internal interfaces => model internal wiring of stuffs

    public class SimpleMDQuoteModel : IQuoteModel
    {
        public event Action<ulong> Updated;

        public uint BidQty { get; private set; }
        public FixedPointDecimal BidPrice { get; private set; }
        public uint AskQty { get; private set; }
        public FixedPointDecimal AskPrice { get; private set; }

        
    }
}
