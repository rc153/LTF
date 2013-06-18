using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.MarketData
{
    public interface ITradeModel
    {
        uint LastQty { get; }
        FixedPointDecimal LastPrice { get; }
    }

    [GuidAttribute("D5E7CDC8-E3DC-4085-88AB-554BCDE5A2C1"), ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITradeModelPublic
    {
        uint LastQty { get; }
        double LastPrice { get; }
    }

    public class AdapterITradeModel : ITradeModelPublic
    {
        private ITradeModel src;

        public AdapterITradeModel(ITradeModel src)
        {
            this.src = src;
        }

        public uint LastQty { get { return src.LastQty; } }
        public double LastPrice { get { return (double)src.LastPrice; } }
    }
}
