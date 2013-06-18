using Disruptor;
using Disruptor.Dsl;
using Disruptor.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.Core;
using Toolkit.Output.Log;
using Toolkit.Output.Perf;
using Toolkit.Threading;

namespace Toolkit.Output
{

    internal class OutputProcessor : IEventHandler<OutputPayload>
    {
        public void OnNext(OutputPayload data, long sequence, bool endOfBatch)
        {
            if ((data.type & MsgType.LOG) == MsgType.LOG)
            {
                LogWriter.OnNext(data);
            }
            else if ((data.type & MsgType.GUI) == MsgType.GUI)
            {
            }
            else if ((data.type & MsgType.PERF) == MsgType.PERF)
            {
            }
            else
            {
                Console.WriteLine("OutputProcessor can't understand message!");
            }
        }
    }

    internal static class Output
    {
        private const int RING_SIZE = 1024;

        private static Disruptor<OutputPayload> disruptor;
        private static RingBuffer<OutputPayload> ringBuffer;

        static Output()
        {
            disruptor = new Disruptor<OutputPayload>(() => new OutputPayload(),
                new MultiThreadedLowContentionClaimStrategy(RING_SIZE),
                   new SleepingWaitStrategy(), new RoundRobinThreadAffinedTaskScheduler(1));
            disruptor.HandleEventsWith(new OutputProcessor());
            ringBuffer = disruptor.Start();
        }

        // use this to flush and stop all output
        public static void Shutdown()
        {
            Logger.Shutdown();
            Thread.Sleep(100);  // wait a bit to make sure all writing is done
            disruptor.Shutdown();
        }

        internal static void DoLog(ulong now, LogLevel level, string format, IRecyclerDynamicElement<ILogContext> context, Exception ex, string sourceFilePath, int sourceLineNumber)
        {
            long sequenceNo = ringBuffer.Next();
            OutputPayload entry = ringBuffer[sequenceNo];
            entry.time = now;
            entry.type = (MsgType)level;
            entry.subject = sourceFilePath;
            entry.format = format;
            entry.context = context;
            entry.ex = ex;
            ringBuffer.Publish(sequenceNo);
        }

        internal static void DoPerf(Time now, SensorType type, string name, uint hits, Time elapsed)
        {
            long sequenceNo = ringBuffer.Next();
            OutputPayload entry = ringBuffer[sequenceNo];
            entry.time = now;
            entry.type = (MsgType)type;
            entry.subject = name;
            entry.hits = hits;
            entry.elapsed = elapsed;
            ringBuffer.Publish(sequenceNo);
        }
    }
}
