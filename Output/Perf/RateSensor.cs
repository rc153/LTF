using System;
using Toolkit.Core;

namespace Toolkit.Output.Perf
{
    // record event rate (emit a log every x seconds or every y events, whichever comes first)
    public class RateSensor
    {
        private Time start = Time.Zero;
        private uint hits = 0;

        public string Name { get; set; }
        public uint ThresholdHits { get; set; }
        public Time ThresholdTime { get; set; }

        public RateSensor(string name, uint thresholdHits = 1, Time thresholdTime = default(Time))
        {
            Name = name;
            ThresholdHits = thresholdHits;
            ThresholdTime = thresholdTime;
        }

        public void Record(Time now)
        {
            if (++hits >= ThresholdHits)
            {
                ulong deltaT = now - start;
                if (deltaT >= ThresholdTime)
                {
                    Output.DoPerf(now, SensorType.RATE, Name, hits, deltaT);
                    hits = 0;
                    start = now;
                }
            }
        }
    }
}
