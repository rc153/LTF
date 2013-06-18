using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.MarketData.SimpleQuote
{
    // todo change all the timestamps to something more logic, ie long of us, or uint of ms
    public class SimpleMDQuoteUpdate
    {
        public ulong Timestamp;
        public uint BidQty;
        public FixedPointDecimal BidPrice;
        public uint AskQty;
        public FixedPointDecimal AskPrice;
        public UpdateType Type;

        internal SimpleMDQuoteModel Model;

        internal void Dispatch(ulong nothing)
        {
        // todo    Model.applyUpdateAndRaiseEvent(this);
        }
    }
}
