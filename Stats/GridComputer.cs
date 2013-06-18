using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Core;
using Toolkit.Utils;

namespace Toolkit.Stats
{
    // can also do a weighted version
    public class GridComputer
    {
        private double returnThreshold;
        private FixedPointDecimal prevPrice;
        private WrappedArray<FixedPointDecimal> prices;
        private WrappedArray<Time> times;

        public GridComputer(int size, double returnThreshold = 0)
        {
            this.returnThreshold = returnThreshold;
            this.prices = new WrappedArray<FixedPointDecimal>(size);
            this.times = new WrappedArray<Time>(size);
        }

        public void update(Time time, FixedPointDecimal newPrice)
        {
            double thisReturn = (double)newPrice / (double)prevPrice - 1;
            if (thisReturn > +returnThreshold || thisReturn < -returnThreshold)
            {
                times.Write(time);
                prices.Write(newPrice);
                prevPrice = newPrice;
            }
        }

        public double getSpeed()
        {
            return prices.Length * returnThreshold / (times.first - times.last);
        }

        public double getDir()
        {
            return Math.Abs(getSignedDir());
        }

        public double getSide()
        {
            return 1 - getDir();
        }

        public double getSignedDir()
        {
            FixedPointDecimal sum = (prices.first - prices.last);
            FixedPointDecimal sumAbs = FixedPointDecimal.Zero;
            for (int i = 0; i < prices.Length; i++)
            {
                sumAbs += prices[i].Abs();
            }
            return (double)sum / (double)sumAbs;
        }
    }
}
