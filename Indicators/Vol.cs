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
    public class Vol : IndicatorBase
    {
        private EwmaComputer avg;
        private OnlineUnivariateStat stat = new OnlineUnivariateStat();

        public override void Initialize()
        {
            Time halfLife = cfg.getTime("time", Time.fromMinutes(5));
            avg = new EwmaComputer(halfLife);

            setDirtyWhenMidChange();
            setDirtyOnTimer(halfLife);
        }

        public override double getValue(Time now)
        {
            double refPrice = avg.update(now, instr.MidPrice.ToDouble());
            stat.Add(refPrice / instr.MidPrice.ToDouble() - 1);
            return stat.StandardDeviation;
        }
    }
}
