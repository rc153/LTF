using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Plugins;

namespace Toolkit.TickSize
{
    public class FixedTickSize : ITickSize, IPlugin
    {
        private FixedPointDecimal value;

        public FixedTickSize(FixedPointDecimal value)
        {
            Init(value);
        }

        // low_tick,more_than,tick,....,max_bound,high_tick
        public void Initialize(Core.IEnvironment env, Configuration.IConfiguration cfg)
        {
            Init(cfg.getFixedPointDecimal("value"));
        }

        private void Init(FixedPointDecimal value)
        {
            this.value = value;
        }

        public FixedPointDecimal roundUp(FixedPointDecimal price)
        {
            return price.RoundUp(value);
        }

        public FixedPointDecimal roundDown(FixedPointDecimal price)
        {
            return price.RoundDown(value);
        }

        public FixedPointDecimal getTickUp(FixedPointDecimal price)
        {
            return value;
        }

        public FixedPointDecimal getTickDown(FixedPointDecimal price)
        {
            return value;
        }

        public FixedPointDecimal getBoundUp(FixedPointDecimal price)
        {
            return FixedPointDecimal.MaxValue;

        }

        public FixedPointDecimal getBoundDown(FixedPointDecimal price)
        {
            return FixedPointDecimal.MinValue;
        }
    }
}
