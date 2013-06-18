using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.TickSize
{
    public interface ITickSize
    {
      //  FixedPointDecimal roundUp(double price);
      //  FixedPointDecimal roundDown(double price);
        FixedPointDecimal roundUp(FixedPointDecimal price);
        FixedPointDecimal roundDown(FixedPointDecimal price);

      //  FixedPointDecimal getTickUp(double price);
      //  FixedPointDecimal getTickDown(double price);
        FixedPointDecimal getTickUp(FixedPointDecimal price);
        FixedPointDecimal getTickDown(FixedPointDecimal price);

       // FixedPointDecimal getBoundUp(double price);
       // FixedPointDecimal getBoundDown(double price);
        FixedPointDecimal getBoundUp(FixedPointDecimal price);
        FixedPointDecimal getBoundDown(FixedPointDecimal price);
    }
}
