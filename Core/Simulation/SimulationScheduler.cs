using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Output.Log;
using Toolkit.MarketData;

namespace Toolkit.Core.Simulation
{
    // todo sync feed threads and manage recycles

    class SimulationScheduler : IScheduler
    {
        private SortedDictionary<Time, Queue<Action<Time>>> queue = new SortedDictionary<Time, Queue<Action<Time>>>();
        private Queue<Queue<Action<Time>>> recylePool = new Queue<Queue<Action<Time>>>();

        public Time Now { get; private set; }

        public void ScheduleAt(Time timestamp, Action<Time> callback)
        {
            if (timestamp <= Now)
                Add(timestamp, callback);
            else
            {
                Add(Now, callback);
                Logger.Log(Now, LogLevel.INFO, "Trying to schedule in the past");
            }
        }

        public void ScheduleAfter(Time timestamp, Action<Time> callback)
        {
            ScheduleAt(Now + timestamp, callback);
        }

        // todo implement a list that doesn't count background stuff
        public void ScheduleAfterBackground(Time timestamp, Action<Time> callback)
        {
            ScheduleAt(Now + timestamp, callback);
        }

        private void Add(Time timestamp, Action<Time> callback)
        {
            Queue<Action<Time>> toRun;
            if (!queue.TryGetValue(timestamp, out toRun))
            {
                if (recylePool.Count > 0)
                    toRun = recylePool.Dequeue();
                else
                    toRun = new Queue<Action<Time>>();
                queue.Add(timestamp, toRun);
            }
            toRun.Enqueue(callback);
        }

        internal void Run()
        {
            while (queue.Count > 0)
            {
                Now = queue.Keys.First();
                Process(Now);
            }
        }

        private void Process(Time timestamp)
        {
            Queue<Action<Time>> toRun;
            if (queue.TryGetValue(timestamp, out toRun))
            {
                while (toRun.Count > 0)
                    toRun.Dequeue()(timestamp);

                toRun.Clear();
                queue.Remove(timestamp);
                recylePool.Enqueue(toRun);
            }
        }
    }
}
