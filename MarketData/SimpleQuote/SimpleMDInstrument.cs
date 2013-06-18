using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Core;
using Toolkit.IO;
using Toolkit.MarketData.SimpleQuote;

namespace Toolkit.MarketData.SimpleQuote
{
    public class SimpleMDInstrument
    {
        private IEnvironment env;
        private CompactBinaryReader theFile;
        internal SimpleMDQuoteModel Model { get; private set; }

        public SimpleMDInstrument(IEnvironment env, CompactBinaryReader theFile)
        {
            this.env = env;
            this.theFile = theFile;

            Model = new SimpleMDQuoteModel();

            Run(0);

           // env.Scheduler.ScheduleAt(timestamp, update.Dispatch);

            // schedule loading of the next update
         //   env.Scheduler.ScheduleAt(timestamp, Run);
        }

        // todo virer le timestamp du call du scheduler, et le laisser dans les updates, car c'est le timstamp auquel on a recu l'update
        // we could do delta coding using the zigzag encoding
        private SimpleMDQuoteUpdate Run(ulong nothing)
        {
            try
            {
                SimpleMDQuoteUpdate update = new SimpleMDQuoteUpdate();
                UpdateType updateType = (UpdateType)theFile.ReadByte();
                ulong timestamp = theFile.ReadUInt64();
                switch (updateType)
                {
                    case UpdateType.QuoteBidQty:
                        {
                            uint qty = theFile.ReadUInt32();
                            update.Model = Model;
                            update.Timestamp = timestamp;
                            update.Type = updateType;
                            update.BidQty = qty;
                            update.Model = Model;
                        }
                        break;
                    case UpdateType.QuoteBidQtyAndPrice:
                        {
                            uint qty = theFile.ReadUInt32();
                            FixedPointDecimal price = theFile.ReadFixedPointDecimal();
                            update.Model = Model;
                            update.Timestamp = timestamp;
                            update.Type = updateType;
                            update.BidQty = qty;
                            update.BidPrice = price;
                            update.Model = Model;
                        }
                        break;
                    case UpdateType.QuoteAskQty:
                        {
                            uint qty = theFile.ReadUInt32();
                            update.Model = Model;
                            update.Timestamp = timestamp;
                            update.AskQty = qty;
                            update.Model = Model;
                        }
                        break;
                    case UpdateType.QuoteAskQtyAndPrice:
                        {
                            uint qty = theFile.ReadUInt32();
                            FixedPointDecimal price = theFile.ReadFixedPointDecimal();
                            update.Model = Model;
                            update.Timestamp = timestamp;
                            update.AskQty = qty;
                            update.AskPrice = price;
                            update.Model = Model;
                        }
                        break;
                    case UpdateType.QuoteAll:
                        {
                            uint bqty = theFile.ReadUInt32();
                            FixedPointDecimal bprice = theFile.ReadFixedPointDecimal();
                            uint aqty = theFile.ReadUInt32();
                            FixedPointDecimal aprice = theFile.ReadFixedPointDecimal();
                            update.Model = Model;
                            update.Timestamp = timestamp;
                            update.Type = updateType;
                            update.BidQty = bqty;
                            update.BidPrice = bprice;
                            update.AskQty = aqty;
                            update.AskPrice = aprice;
                            update.Model = Model;
                        }
                        break;
                    default:
                        throw new FormatException("Unexpected token in the feed");
                }

                return update;
            }
            catch (EndOfStreamException)
            {
                // done, nothing to do anymore
                theFile.Close();
                return null;
            }
        }
    }
}
