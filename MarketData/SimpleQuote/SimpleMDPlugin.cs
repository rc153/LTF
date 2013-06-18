using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolkit.Core;
using Toolkit.IO;
using Toolkit.MarketData.SimpleQuote;

namespace Toolkit.MarketData
{
    public class SimpleMDPlugin : MDPluginBase
    {
        protected override SimpleMDQuoteModel CreateQuoteModel(IEnvironment env, CompactBinaryReader reader)
        {
            return (new SimpleMDInstrument(env, reader)).Model;
        }
    }
}
