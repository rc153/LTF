using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.MarketData
{
    public enum UpdateType : byte
    {
        QuoteBidQty,
        QuoteBidQtyAndPrice,
        QuoteAskQty,
        QuoteAskQtyAndPrice,
        QuoteAll,

        TradeQty,
        TradeQtyAndPrice,

        BookBid,
        BookAsk,

        OrderBid,
        OrderAsk
    }
}
