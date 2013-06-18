using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Ids;
using Toolkit.MarketData;
using Toolkit.Plugins;
using Toolkit.TickSize;

namespace Toolkit.Book
{
    public interface IUniverseService
    {
        Instrument GetInstrument(Id id);
    }

    // create book and instruments based on index, or csv
    public class IndexUniverse : IPlugin
    {
        private IEnvironment env;
        private IIdService idService;
        private SymbolType symbolType;
        private IConfiguration mainCfg;
        private InstrumentsConfiguration instrDefinitions;

        public void Initialize(IEnvironment env, IConfiguration cfg)
        {
            this.env = env;
            idService = env.GetIdService();
            symbolType = SymbolType.Parse(cfg.getString("symbolType"));

            mainCfg = cfg;
            Read(cfg.getString("source"));
        }

        private void Read(string filename)
        {
            instrDefinitions = new InstrumentsConfiguration();
            instrDefinitions.Read(filename);
        }

        public Instrument GetInstrument(Id id)
        {
            throw new NotImplementedException();
        }

        public Instrument[] Create()
        {
            Instrument[] result = new Instrument[instrDefinitions.SymbolCount];
            int i = 0;
            foreach (string symbol in instrDefinitions.Symbols)
            {
                result[i++] = Create(symbol);
            }
            return result;
        }

        private Instrument Create(string symbol)
        {
            Id id = idService.GetId(symbol, symbolType);
            return new Instrument((IQuoteFeed)env.GetPlugin(mainCfg.getString("quote")),
                (ITradeFeed)env.GetPlugin(mainCfg.getString("trade")))
            {
                id = id,
                multiplier = mainCfg.getDoubleForInstrument("multiplier", instrDefinitions, symbol, 1),
                quotity = (int)mainCfg.getDoubleForInstrument("lotsize", instrDefinitions, symbol, 1),     // todo could be variable like korea/japan
                tick = GetTickSize(mainCfg.getStringForInstrument("tick", instrDefinitions, symbol)),
                strike = mainCfg.getFixedPointDecimalForInstrument("strike", instrDefinitions, symbol, FixedPointDecimal.Zero),
            };
        }

        private ITickSize GetTickSize(string def)
        {
            if (def.Split(',').Length > 1)
            {
                return new RuleTickSize(def);
            }
            return new FixedTickSize(FixedPointDecimal.Parse(def));
        }
    }
}
