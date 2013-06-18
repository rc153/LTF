using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.MarketData;

namespace Toolkit.Core
{
    public interface IScheduler
    {
        // todo do we really need to pass the time?
        void ScheduleAt(Time timestamp, Action<Time> callback);
        void ScheduleAfter(Time delay, Action<Time> callback);
        void ScheduleAfterBackground(Time delay, Action<Time> callback);
    }
}
