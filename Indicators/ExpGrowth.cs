using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Ids;
using Toolkit.Plugins;
using Toolkit.Stats;

namespace Toolkit.Indicators
{
    // todo fit an exp growth model and if R2 is large enough try to do breakout
    // this would be sweet using the multi scale thing
    public class ExpGrowth : IndicatorBase
    {
        private GridComputer gc;

        public override void Initialize()
        {
            int size = (int)cfg.getDouble("size", 10);
            double returnThreshold = cfg.getDouble("grid", 0.01 / 50);
            gc = new GridComputer(size, returnThreshold);
            midChanged += Speed_midChanged;
        }

        void Speed_midChanged(Time time)
        {
            gc.update(time, instr.MidPrice);
        }

        public override double getValue(Time now)
        {
            return gc.getSpeed();
        }
    }
}
