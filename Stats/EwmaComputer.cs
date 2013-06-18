using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolkit.Core;

namespace Toolkit.Indicators
{
    public interface InitStrategy : IResetable
    {
        double update(Time deltaT, double newX);
        bool Done { get; }
    }

    public class ThirdMeanInitStragey : InitStrategy
    {
        private readonly Time halfLife;
        private double sumX = 0;
        private double sumT = 0;

        public ThirdMeanInitStragey(Time halfLife)
        {
            this.halfLife = halfLife;
        }

        public double update(Time deltaT, double newX)
        {
            if (3 * sumT > halfLife)
                Done = true;
            sumX += deltaT * newX;
            sumT += deltaT;
            return sumX / sumT;
        }

        public void Reset()
        {
            sumX = 0;
            sumT = 0;
        }

        public bool Done { get; private set; }
    }

    // todo move
    // todo fpd?
    public class EwmaComputer : IIsValid, IResetable
    {
        private Time lastTime;
        private double lastX;
        private readonly Time halfLife;
        private readonly Time minDeltaT;
        private double value;
        private InitStrategy initStrat;

        public EwmaComputer(Time halfLife)
            : this(halfLife, new ThirdMeanInitStragey(halfLife))
        { }

        public EwmaComputer(Time halfLife, InitStrategy initStrat)
        {
            this.halfLife = halfLife;
            this.minDeltaT = halfLife / 100UL;
            this.initStrat = initStrat;
        }

        public double update(Time now, double newX)
        {
            Time deltaT = now - lastTime;
            double deltaX = newX - lastX;

            if (deltaX * 1e6 > newX || deltaT > minDeltaT)
            {
                lastX = newX;
                lastTime = now;
                if (!initStrat.Done)
                {
                    value = initStrat.update(deltaT, newX);
                }
                else
                {
                    double alpha = Math.Exp(-(double)deltaT / halfLife);
                    value = alpha * value + (1 - alpha) * newX;
                }
            }
            return value;
        }

        public double getValue(Time now)
        {
            return update(now, lastX);
        }

        public bool IsValid
        {
            get { return initStrat.Done; }
        }

        public void Reset()
        {
            initStrat.Reset();
        }
    }
}
