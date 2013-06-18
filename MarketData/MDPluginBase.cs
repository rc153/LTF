
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Ids;
using Toolkit.IO;
using Toolkit.MarketData.Backend;
using Toolkit.MarketData.SimpleQuote;
using Toolkit.Plugins;

namespace Toolkit.MarketData
{
    // todo simple update pool and recycle between threads
    // todo read in another thread, and secure the scheduler with a lock
    // todo or use the disruptor
    // all of that will need to spearate this in a reader, a writer, model, update, sim service, realtime service (remember the backend / front end thingy)
    // front end provides the model, while backend gives it the stream, see who calls who because there's thread implication there
    // the getReader call on the backend must be executed by the feed thread, while the call to create model doesn't matter so much
    public abstract class MDPluginBase : IQuoteFeed, IPlugin
    {
        private IEnvironment env;
        private IIdService idService;
        private SymbolType symbolType;
        private Dictionary<Id, IQuoteModel> instruments = new Dictionary<Id, IQuoteModel>();
        private IMDBackend backend;

        public void Initialize(IEnvironment env, IConfiguration cfg)
        {
            this.env = env;

            idService = env.GetIdService();
            symbolType = SymbolType.Parse(cfg.getString("symbolType"));

            backend = (IMDBackend)env.LoadPlugin(cfg.SubSet("be"));
        }

        public IQuoteModel getOrCreateModel(Id id)
        {
            IQuoteModel instr;
            if (!instruments.TryGetValue(id, out instr))
            {
                string symbol = idService.GetSymbol(id, symbolType);
                instr = CreateQuoteModel(env, backend.getReader(symbol));
                instruments.Add(id, instr);
            }
            return instr;
        }   

        protected abstract SimpleMDQuoteModel CreateQuoteModel(IEnvironment env, CompactBinaryReader reader);
    }
}
