using System.Diagnostics;
using Toolkit.Core;
using Toolkit.Stats;

namespace Toolkit.Output.Perf
{
    // use cases:
    // - record durations (emit a log every run that is over mean + x sigma)
    // - record event rate (emit a log every x seconds and every y events)
    // - measure the size of a queue ?

    public enum SensorType : byte
    {
        DURATION = MsgType.PERF + 0,
        RATE = MsgType.PERF + 1,
    }

    // record durations (emit a log every run that is over mean + x sigma)
    public class DurationSensor
    {
        private readonly static double freq;
        private long timeStamp;
        private OnlineUnivariateStat stat = new OnlineUnivariateStat();

        public string Name { get; set; }
        public double ThresholdSigma { get; set; }

        static DurationSensor()
        {
            freq = (double)Time.ticksPerSecond / Stopwatch.Frequency;
        }

        public DurationSensor(string name, double thresholdSigma = 1)
        {
            Name = name;
            ThresholdSigma = thresholdSigma;
        }

        public void BeginRecord()
        {
            timeStamp = Stopwatch.GetTimestamp();
        }

        public void EndRecord(Time now)
        {
            long duration = Stopwatch.GetTimestamp() - timeStamp;
            stat.Add(duration);

            if (duration > stat.Mean + ThresholdSigma * stat.StandardDeviation)
            {
                Time durationNs = new Time(unchecked((ulong)(duration * freq)));
                Output.DoPerf(now, SensorType.DURATION, Name, 1, durationNs);
            }
        }
    }
}
