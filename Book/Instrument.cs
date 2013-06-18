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
    // i think it was acutally quite nice not to have 2 different classes for futures and stocks?
    // must support options as well, 
    public partial class Instrument
    {
        public Instrument(IQuoteFeed quoteFeed, ITradeFeed tradeFeed)
        {
            throw new NotImplementedException();
        }

        public Id id { get; internal set; }
        public double multiplier { get; internal set; }
        public int quotity { get; internal set; }
        public ITickSize tick { get; internal set; }
        public FixedPointDecimal strike { get; internal set; }
    }
}
