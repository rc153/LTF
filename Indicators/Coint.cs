using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Book;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Ids;
using Toolkit.Plugins;
using Toolkit.Stats;
using Toolkit.Utils;

namespace Toolkit.Indicators
{
    public class Coint : IndicatorBase
    {
        private Time sampleTime;
        private WrappedArray<FixedPointDecimal> prices1;
        private WrappedArray<FixedPointDecimal> prices2;

        private Instrument instr2;

        public override void Initialize()
        {
            int size = (int)cfg.getDouble("size", 10);
            sampleTime = Time.fromSeconds(cfg.getDouble("timeSec", 5 * 60));

            prices1 = new WrappedArray<FixedPointDecimal>(size);
            prices2 = new WrappedArray<FixedPointDecimal>(size);

            Id id2 = env.GetIdService().GetId(cfg.getString("ref"), cfg.getEnum<SymbolType>("symbolType"));
            instr2 = env.GetUniverseService().GetInstrument(id2);

            env.Scheduler.ScheduleAfterBackground(sampleTime, sample);
        }

        private void sample(Time time)
        {
            prices1.Write(instr.MidPrice);
            prices2.Write(instr2.MidPrice);
            env.Scheduler.ScheduleAfterBackground(sampleTime, sample);
        }

        public override double getValue(Time now)
        {
            double sum = 0;
            for (int i = 1; i < prices1.Length; i++)
            {
                sum += Math.Sign((double)(prices1[i] - prices1[i - 1]) * (double)(prices2[i] - prices2[i - 1]));
            }
            return sum / prices1.Length;
        }
    }
}
