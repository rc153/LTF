using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Core;
using Toolkit.Ids;
using Toolkit.MarketData;
using Toolkit.MarketData.SimpleQuote;
using Toolkit.Position;
using Toolkit.TickSize;

namespace Toolkit.Book
{
    // todo get the feed infra to update this directly
    public partial class Instrument : IQuoteModel, ITradeModel
    {
        public event Action<Time> midChanged;
        public event Action<Time> quoteChanged;
        public event Action<Time> tradeChanged;
        public event Action<Time> bookChanged;

        public uint BidQty { get; internal set; }
        public FixedPointDecimal BidPrice { get; internal set; }
        public uint AskQty { get; internal set; }
        public FixedPointDecimal AskPrice { get; internal set; }
        public FixedPointDecimal MidPrice { get; internal set; }

        public uint LastQty { get; internal set; }
        public FixedPointDecimal LastPrice { get; internal set; }

        internal void applyUpdateAndRaiseEvent(SimpleMDQuoteUpdate update)
        {
            switch (update.Type)
            {
                case UpdateType.QuoteBidQty:
                    BidQty = update.BidQty;
                    break;
                case UpdateType.QuoteBidQtyAndPrice:
                    BidQty = update.BidQty;
                    BidPrice = update.BidPrice;
                    break;
                case UpdateType.QuoteAskQty:
                    AskQty = update.AskQty;
                    break;
                case UpdateType.QuoteAskQtyAndPrice:
                    AskQty = update.AskQty;
                    AskPrice = update.AskPrice;
                    break;
                case UpdateType.QuoteAll:
                    BidQty = update.BidQty;
                    BidPrice = update.BidPrice;
                    AskQty = update.AskQty;
                    AskPrice = update.AskPrice;
                    break;
                default:
                    throw new FormatException("Unexpected UpdateType in SimpleQuoteModel");
            }

           // todo Updated(update.Timestamp);
        }
    }
}
