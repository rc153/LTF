using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Toolkit.Core
{
    // todo
    public struct Time
    {
        public const ulong ticksPerHour = 60L * 60L * 1000L * 1000L * 10L;
        public const ulong ticksPerMinute = 60L * 1000L * 1000L * 10L;
        public const ulong ticksPerSecond = 1000L * 1000L * 10L;
        public const ulong ticksPerMillisecond = 1000L * 10L;
        public const ulong ticksPerMicrosecond = 10L;

        public static readonly Time Zero = new Time(0);

        private ulong time;

        public Time(ulong time)
        {
            this.time = time;
        }

        public static Time fromSeconds(double sec)
        {
            return new Time((ulong)(sec * ticksPerSecond));
        }

        public static Time fromMinutes(double min)
        {
            return new Time((ulong)(min * ticksPerMinute));
        }

        public static implicit operator ulong(Time src)
        {
            return src.time;
        }

        public static implicit operator Time(ulong src)
        {
            return new Time(src);
        }

        public static Time operator -(Time one, Time other)
        {
            return new Time(one.time - other.time);
        }

        public static Time operator +(Time one, Time other)
        {
            return new Time(one.time + other.time);
        }

        public static Time operator /(Time one, Time other)
        {
            return new Time(one.time / other.time);
        }

        public static Time operator /(Time one, ulong other)
        {
            return new Time(one.time / other);
        }
    }

    // not static on purpose not to think it would be thread safe
    // todo do thread safe with an internal instance we replace after reset
    // and reuse the old one for next time
    // and make static again
    public class HiResDateTime
    {


        private const int ADJUST_SIZE = 33;

        private readonly static long today;
        private readonly static long tzOffset;
        private readonly static double freq;
        private readonly static ulong maxBeforeReset = 10 * (ulong)Time.ticksPerSecond;

        private ulong midnightSWTicks;
        private ulong lastResult;
        private long lastSWTimestamp;
        private ulong lastAdjustSWTIcks;

        static HiResDateTime()
        {
            today = DateTime.Today.Ticks;
            tzOffset = DateTime.Now.Ticks - DateTime.UtcNow.Ticks;
            freq = (double)Time.ticksPerSecond / Stopwatch.Frequency;
        }

        public HiResDateTime()
        {
            // warming up
            Adjust(null);
        }

        public ulong NowTicks
        {
            get
            {
                // cache
                long nowSWTimeStamp = Stopwatch.GetTimestamp();
                if (nowSWTimeStamp == lastSWTimestamp)
                    return lastResult;
                lastSWTimestamp = nowSWTimeStamp;

                // measure in tick space
                ulong nowSWTicks = unchecked((ulong)(nowSWTimeStamp * freq));

                // if we didn't reset for some time
                if (nowSWTicks - lastAdjustSWTIcks > maxBeforeReset)
                {
                    ThreadPool.UnsafeQueueUserWorkItem(Adjust, null);
                    lastAdjustSWTIcks = nowSWTicks;
                }

                // ticks from midnight local time
                ulong result = nowSWTicks - midnightSWTicks;

                // don't go back in the past
                if (result > lastResult)
                    lastResult = result;
                return lastResult;
            }
        }

        private void Adjust(object nothing)
        {
            ulong[] adjustValues = new ulong[ADJUST_SIZE];

            for (int adjustOffset = 0; adjustOffset < ADJUST_SIZE; adjustOffset++)
            {
                long adjustTicks = DateTime.UtcNow.Ticks;
                long lastAdjustDTTicks = adjustTicks;
                while (lastAdjustDTTicks == adjustTicks)
                    adjustTicks = DateTime.UtcNow.Ticks;

                long nowSWTimeStamp = Stopwatch.GetTimestamp();
                long nowSWTicks = unchecked((long)(nowSWTimeStamp * freq));

                adjustValues[adjustOffset] = unchecked((ulong)(today + nowSWTicks - (adjustTicks + tzOffset)));
            }

            // compute trimean
            Array.Sort(adjustValues);
            midnightSWTicks = (adjustValues[ADJUST_SIZE / 2 - 1] + adjustValues[ADJUST_SIZE / 2] + adjustValues[ADJUST_SIZE / 2 + 1]) / 3;
        }

        public ulong NowMicroseconds
        {
            get
            {
                return NowTicks / 10;
            }
        }

        public ulong NowMilliseconds
        {
            get
            {
                return NowTicks / 10000;
            }
        }

        public static string Format(long ticks)
        {
            return Format(unchecked((ulong)ticks));
        }

        public static string Format(ulong ticks)
        {
            // ticks -= today;
            ulong hour = ticks / Time.ticksPerHour;
            ticks -= hour * Time.ticksPerHour;
            ulong min = ticks / Time.ticksPerMinute;
            ticks -= min * Time.ticksPerMinute;
            ulong sec = ticks / Time.ticksPerSecond;
            ticks -= sec * Time.ticksPerSecond;
            ulong micro = ticks / Time.ticksPerMicrosecond;
            return String.Format("{0}:{1}:{2}.{3:000000}", hour, min, sec, micro);
        }
    }
}
