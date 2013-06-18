using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.FSM;

namespace Toolkit.Risk
{
    // todo http://tr8dr.wordpress.com/2012/09/29/money-management/
    public class MoneyManager
    {
        private enum States
        {
            Trading,
            WaitReversal,
            WaitPenalty
        }

        private EventFSM<States> fsm;
        private Action stopOut = null;
        private Action driftOut = null;
        private Action profitOut = null;
        private Action timeout = null;
        private Action reversed = null;

        public MoneyManager()
        {
            fsm = new EventFSM<States>(States.Trading);

            fsm.In(States.Trading).On(ref stopOut).Goto(States.WaitPenalty);
            fsm.In(States.Trading).On(ref driftOut).Goto(States.WaitReversal);
            fsm.In(States.Trading).On(ref profitOut).Goto(States.WaitReversal);

            fsm.In(States.WaitPenalty).On(ref timeout).Goto(States.Trading);
            fsm.In(States.WaitReversal).On(ref reversed).Goto(States.Trading);
        }
    }

    // map a [-1,1] signal to position and emit trading
    // this is probably a poor way of doing things,
    // you'd better allocate your size based on a metric based both on signal strength and
    // incertitude of the prediction
    public class SignalToPosition
    {
        private double maxPosition;
        private double tradingSize;

        private double currentPosition;

        public SignalToPosition(double maxPosition) : this(maxPosition, maxPosition / 5) { }

        public SignalToPosition(double maxPosition, double tradingSize)
        {
            this.maxPosition = maxPosition;
            this.tradingSize = tradingSize;
        }

        // todo get a proper event in and sync the position as well
        public double Process(double signal)
        {
            double stepSize = tradingSize / maxPosition;
            double minSig = (currentPosition - tradingSize) * stepSize;
            double maxSig = (currentPosition + tradingSize) * stepSize;
            if (signal < minSig) return -tradingSize;
            if (signal > maxSig) return +tradingSize;
            return 0;
        }
    }
}
