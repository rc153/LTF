using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Stats
{
    public static class R
    {
        public static double[] seq(double begin, double end, double inc = 1)
        {
            int n = (int)((end - begin + 1) / inc);
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = begin + i * inc;
            }
            return result;
        }

        public static T[] rep<T>(T value, uint times)
        {
            T[] result = new T[times];
            for (int i = 0; i < times; i++)
            {
                result[i] = value;
            }
            return result;
        }

        public static T[] c<T>(params T[] values)
        {
            return values;
        }
    }
}
