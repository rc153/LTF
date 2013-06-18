using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Plugins;

namespace Toolkit.TickSize
{
    public class RuleTickSize : ITickSize, IPlugin
    {
        private FixedPointDecimal[] bounds;
        private FixedPointDecimal[] values;

        public RuleTickSize(string rules)
        {
            Init(rules);
        }

        // low_tick,more_than,tick,....,max_bound,high_tick
        public void Initialize(Core.IEnvironment env, Configuration.IConfiguration cfg)
        {
            Init(cfg.getString("rules"));
        }

        private void Init(string rules)
        {
            values = (from rule in rules.Split(',').Odd()
                      select FixedPointDecimal.Parse(rule)).ToArray();
            bounds = (from rule in rules.Split(',').Even()
                      select FixedPointDecimal.Parse(rule)).ToArray();
        }

        private int binarySearch(FixedPointDecimal price, int equal)
        {
            int low = 0;
            int high = bounds.Length - 1;
            while (low <= high)
            {
                int mid = low + ((high - low) >> 1);
                int order = price.CompareTo(bounds[mid]);
                if (order == 0) return mid + equal;
                if (order < 0) high = mid - 1;
                else low = mid + 1;
            }
            return low;
        }

        private int findIndexOfUp(FixedPointDecimal price)
        {
            return binarySearch(price, 1);

            /* for (int i = 0; i < bounds.Length; i++)
             {
                 if (price < bounds[i]) return i;
             }
             return bounds.Length; */
        }

        private int findIndexOfDown(FixedPointDecimal price)
        {
            return binarySearch(price, 0);

            /* for (int i = 0; i < bounds.Length; i++)
             {
                 if (price <= bounds[i]) return i;
             }
             return bounds.Length; */
        }

        public FixedPointDecimal roundUp(FixedPointDecimal price)
        {
            int i = findIndexOfUp(price);
            return price.RoundUp(values[i]);
        }

        public FixedPointDecimal roundDown(FixedPointDecimal price)
        {
            int i = findIndexOfDown(price);
            return price.RoundDown(values[i]);
        }

        public FixedPointDecimal getTickUp(FixedPointDecimal price)
        {
            int i = findIndexOfUp(price);
            return values[i];
        }

        public FixedPointDecimal getTickDown(FixedPointDecimal price)
        {
            int i = findIndexOfDown(price);
            return values[i];
        }

        public FixedPointDecimal getBoundUp(FixedPointDecimal price)
        {
            int i = findIndexOfUp(price);
            if (i == bounds.Length) return FixedPointDecimal.MaxValue;
            return bounds[i];
        }

        public FixedPointDecimal getBoundDown(FixedPointDecimal price)
        {
            int i = findIndexOfDown(price) - 1;
            if (i == -1) return FixedPointDecimal.MinValue;
            return bounds[i];
        }
    }
}
