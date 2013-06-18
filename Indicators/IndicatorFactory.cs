using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Ids;
using Toolkit.Plugins;

namespace Toolkit.Indicators
{
    internal class IndicatorFactory
    {
        private static Dictionary<IPlugin, IndicatorFactory> allIndics = new Dictionary<IPlugin, IndicatorFactory>();

        private Dictionary<Id, IndicatorBase> indicsById = new Dictionary<Id, IndicatorBase>();
        private Type pluginType;

        internal static IndicatorBase getOrCreate(IPlugin plugin, Id id)
        {
            IndicatorFactory result;
            if (!allIndics.TryGetValue(plugin, out result))
            {
                result = new IndicatorFactory(plugin);
                allIndics.Add(plugin, result);
            }
            return result.getOrCreate(id);
        }

        private IndicatorFactory(IPlugin plugin)
        {
            pluginType = plugin.GetType();
        }

        private IndicatorBase getOrCreate(Id id)
        {
            IndicatorBase result;
            if (!indicsById.TryGetValue(id, out result))
            {
                result = Create(id);
                indicsById.Add(id, result);
            }
            return result;
        }

        private IndicatorBase Create(Id id)
        {
            IndicatorBase result = (IndicatorBase)Activator.CreateInstance(pluginType, true);
           // result.Initialize(id);
            return result;
        }
    }
}
