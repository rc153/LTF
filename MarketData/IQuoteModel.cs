using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.MarketData
{
    // this doesn't work, so can't use this trick as long as FixedPointDecimal is a value type
    /* public interface foo<out T>
     {
         T a { get; }
     }

     public interface ibar
     {
     }

     public struct bar : ibar
     {
     }

     public class testclass
     {
         public void test(foo<bar> a)
         {
             foo<ibar> b = a;   // does not compile
         }
     }*/

    public interface IQuoteModel
    {
        uint BidQty { get; }
        FixedPointDecimal BidPrice { get; }
        uint AskQty { get; }
        FixedPointDecimal AskPrice { get; }
    }

    [GuidAttribute("15EDCA9F-A46D-443A-9611-4A62E8A41CA6"), ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IQuoteModelPublic
    {
        uint BidQty { get; }
        double BidPrice { get; }
        uint AskQty { get; }
        double AskPrice { get; }
    }

    public class AdapterIQuoteModel : IQuoteModelPublic
    {
        private IQuoteModel src;

        public AdapterIQuoteModel(IQuoteModel src)
        {
            this.src = src;
        }

        public uint BidQty { get { return src.BidQty; } }
        public double BidPrice { get { return (double)src.BidPrice; } }
        public uint AskQty { get { return src.AskQty; } }
        public double AskPrice { get { return (double)src.AskPrice; } }
    }
}
