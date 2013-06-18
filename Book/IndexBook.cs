using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Plugins;

namespace Toolkit.Book
{
    public class IndexBook : InstrumentCollection, IPlugin
    {
        private double[] weights;
        private Instrument[] cashInstrs;
        private Instrument[] derivInstrs;
        private Index.IndexComputer index;

        private double[] redisualPositions; // todo move to instrument
        private double delta;

        private double cashPosition;
        private double derivPosition;
        private double cashNtnl;
        private double derivNtnl;

        public void Initialize(IEnvironment env, IConfiguration cfg)
        {
            IndexUniverse universe = (IndexUniverse)env.GetPlugin(cfg.getString("universe"));
            instrs = universe.Create();
        }

        // todo compute implied correl
        public void ComputeDeltaHedged()
        {
            cashPosition = 0;
            derivPosition = 0;
            cashNtnl = 0;
            derivNtnl = 0;

            for (int i = 0; i < derivInstrs.Length; i++)
            {
                Instrument deriv = derivInstrs[i];
                derivPosition += deriv.Position * deriv.MidPrice.ToDouble() * deriv.multiplier;
                derivNtnl += deriv.TradedNtnl;
            }

            for (int i = 0; i < cashInstrs.Length; i++)
            {
                Instrument cash = cashInstrs[i];
                cashPosition += cash.Nominal;
                cashNtnl += cash.TradedNtnl;
            }

            for (int i = 0; i < cashInstrs.Length; i++)
            {
                Instrument cash = cashInstrs[i];
                redisualPositions[i] = cash.Nominal - cashPosition * weights[i] * cash.MidPrice.ToDouble() / index.Mid;
                // todo compute&cache that somewhere
            }

            delta = cashPosition + derivPosition;
        }

        public void ComputeFlat()
        {
            cashPosition = 0;
            derivPosition = 0;
            cashNtnl = 0;
            derivNtnl = 0;

            for (int i = 0; i < derivInstrs.Length; i++)
            {
                Instrument deriv = derivInstrs[i];
                derivPosition += deriv.Position * deriv.MidPrice.ToDouble() * deriv.multiplier;
                derivNtnl += deriv.TradedNtnl;
            }

            for (int i = 0; i < cashInstrs.Length; i++)
            {
                Instrument cash = cashInstrs[i];
                cashPosition += cash.Nominal;
                cashNtnl += cash.TradedNtnl;

                redisualPositions[i] = cash.Nominal - derivPosition * weights[i] * cash.MidPrice.ToDouble() / index.Mid;
            }

            delta = cashPosition + derivPosition;
        }
    }
}
