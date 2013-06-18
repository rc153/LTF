using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Book;
using Toolkit.Configuration;
using Toolkit.Ids;
using Toolkit.Plugins;

namespace Toolkit.Core.Simple
{
    public class SimpleEnvironment : IEnvironment
    {
        private PluginLoader plugins = new PluginLoader();

        public IPlugin GetPlugin(string pluginName)
        {
            return plugins.Get(pluginName);
        }

        public IPlugin LoadPlugin(IConfiguration cfg)
        {
            return plugins.Load(this, cfg);
        }

        public IIdService GetIdService()
        {
            throw new NotImplementedException();
        }

        public IUniverseService GetUniverseService()
        {
            throw new NotImplementedException();
        }

        public bool IsRunning
        {
            get { throw new NotImplementedException(); }
        }

        public IScheduler Scheduler
        {
            get { throw new NotImplementedException(); }
        }

        public void Run(FileConfiguration cfg, Action entryPoint)
        {
            plugins.LoadAll(this, cfg.SubSet("plugin"));

            foreach (IPlugin plugin in plugins)
                if (plugin is IEnvironmentListener)
                    ((IEnvironmentListener)plugin).EnvironmentOpening();

            entryPoint();

            foreach (IPlugin plugin in plugins)
                if (plugin is IEnvironmentListener)
                    ((IEnvironmentListener)plugin).EnvironmentClosing();
        }
    }
}
