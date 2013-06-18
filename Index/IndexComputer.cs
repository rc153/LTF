using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Book;
using Toolkit.MarketData;

namespace Toolkit.Index
{
    // compute deltas, full refresh every minute
    // todo can we make the weight an FPD?
    public class IndexComputer
    {
        private IndexComputerInstrument[] data;

        public double Bid { get; private set; }
        public double Ask { get; private set; }
        public double Mid { get; private set; }
        public double Last { get; private set; }

        public event Action<ulong> Updated;

        public IndexComputer(Instrument[] instrs, double[] weights)
        {
            Contract.Requires(instrs.Length == weights.Length);

            data = new IndexComputerInstrument[instrs.Length];
            for (int i = 0; i < instrs.Length; i++)
            {
                data[i++] = new IndexComputerInstrument(this, instrs[i], instrs[i], weights[i]);
            }
        }

        public void Refresh(ulong time)
        {
            Bid = Ask = Last = 0;
            foreach (var instr in data)
            {
                instr.Refresh();
            }
            Mid = 0.5 * (Bid + Ask);
            Updated(time);
        }

        // todo remove the double quote/trade thingy
        private class IndexComputerInstrument
        {
            private IndexComputer parent;
            private double weight;
            private IQuoteModel quote;
            private FixedPointDecimal oldBid = FixedPointDecimal.Zero;
            private FixedPointDecimal oldAsk = FixedPointDecimal.Zero;
            private ITradeModel trade;
            private FixedPointDecimal oldLast = FixedPointDecimal.Zero;

            public IndexComputerInstrument(IndexComputer parent, IQuoteModel quote, ITradeModel trade, double weight)
            {
                this.parent = parent;
                this.quote = quote;
                this.trade = trade;
                this.weight = weight;
            }

            public void Refresh()
            {
                parent.Bid += weight * quote.BidPrice.ToDouble();
                parent.Ask += weight * quote.AskPrice.ToDouble();
                parent.Last += weight * trade.LastPrice.ToDouble();
            }

            public void QuoteUpdated(ulong time)
            {
                FixedPointDecimal deltaBid = quote.BidPrice - oldBid;
                FixedPointDecimal deltaAsk = quote.AskPrice - oldAsk;
                parent.Bid += weight * deltaBid.ToDouble();
                parent.Ask += weight * deltaAsk.ToDouble();
                parent.Mid = 0.5 * (parent.Bid + parent.Ask);
                parent.Updated(time);
            }

            public void TradeUpdated(ulong time)
            {
                FixedPointDecimal deltaLast = trade.LastPrice - oldLast;
                parent.Last += weight * deltaLast.ToDouble();
                parent.Updated(time);
            }
        }
    }
}
