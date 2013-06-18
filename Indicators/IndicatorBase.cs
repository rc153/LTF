using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Book;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Ids;
using Toolkit.Plugins;

namespace Toolkit.Indicators
{
    // the factory/plugin in the same class as indicators is a bit strange, but allows to simplify config files
    public abstract class IndicatorBase : IPlugin
    {
        protected IEnvironment env;
        protected IConfiguration cfg;
        protected Instrument instr;

        private bool dirty;

        public void Initialize(IEnvironment env, IConfiguration cfg)
        {
            this.env = env;
            this.cfg = cfg;
        }

        public IndicatorBase getOrCreate(Id id)
        {
            Contract.Requires(instr == null, "use this method only on instances created without instrument, to be used as factories");
            IndicatorBase result = IndicatorFactory.getOrCreate(this, id);
            result.Initialize(id);
            return result;
        }

        private void Initialize(Id id)
        {
            instr = env.GetUniverseService().GetInstrument(id);
            Initialize();
        }

        protected event Action<Time> midChanged
        {
            add { instr.midChanged += setDirty; instr.midChanged += value; }
            remove { instr.midChanged -= setDirty; instr.midChanged -= value; }
        }
        protected event Action<Time> quoteChanged
        {
            add { instr.quoteChanged += setDirty; instr.quoteChanged += value; }
            remove { instr.quoteChanged -= setDirty; instr.quoteChanged -= value; }
        }
        protected event Action<Time> tradeChanged
        {
            add { instr.tradeChanged += setDirty; instr.tradeChanged += value; }
            remove { instr.tradeChanged -= setDirty; instr.tradeChanged -= value; }
        }
        protected event Action<Time> bookChanged
        {
            add { instr.bookChanged += setDirty; instr.bookChanged += value; }
            remove { instr.bookChanged -= setDirty; instr.bookChanged -= value; }
        }

        // if you will have a new value after an event, but don't care to receive all updates, use these
        protected void setDirtyOnTimer(Time halfLife)
        {
            // todo check the halflife here
            instr.quoteChanged += setDirty;
        }

        protected void setDirtyWhenMidChange()
        {
            instr.midChanged += setDirty;
        }
        protected void setDirtyWhenQuoteChange()
        {
            instr.quoteChanged += setDirty;
        }
        protected void setDirtyWhenTradeChange()
        {
            instr.tradeChanged += setDirty;
        }
        protected void setDirtyWhenBookChange()
        {
            instr.bookChanged += setDirty;
        }

        // todo think about carefullt about the dirty model here
        // todo propagate dirty as events
        private void setDirty(Time time)
        {
            dirty = true;
        }

        public abstract void Initialize();

        public abstract double getValue(Time now);
    }
}
