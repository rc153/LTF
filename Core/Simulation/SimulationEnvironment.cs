using System;
using Toolkit.Book;
using Toolkit.Configuration;
using Toolkit.Ids;
using Toolkit.Output.Log;
using Toolkit.Plugins;

namespace Toolkit.Core.Simulation
{
    // today / now ??
    // millis since midnight
    // millis since epoch
    // formatable time
    // timezone
    // load plugins
    // todo do the multi env thing with shared flags

    public class SimulationEnvironment : IEnvironment
    {
        private DateTime today;
        private SimulationScheduler scheduler = new SimulationScheduler();
        private PluginLoader plugins = new PluginLoader();

        public DateTime Today { get { return today; } }
        public ulong Now { get { return scheduler.Now; } }

        public IScheduler Scheduler { get { return scheduler; } }
        public bool IsRunning { get; private set; }

        public IIdService GetIdService()
        {
            return (IIdService)plugins.Get("__ID_SERVICE__");
        }

        public IUniverseService GetUniverseService()
        {
            return (IUniverseService)plugins.Get("__UNIVERSE_SERVICE__");
        }

        public IPlugin GetPlugin(string pluginName)
        {
            return plugins.Get(pluginName);
        }

        public IPlugin LoadPlugin(IConfiguration cfg)
        {
            return plugins.Load(this, cfg); 
        }

        internal void Run(FileConfiguration cfg)
        {
            today = DateTime.Parse(cfg.getString("env.date"));

            plugins.LoadAll(this, cfg.SubSet("plugin"));

            foreach (IPlugin plugin in plugins)
                if (plugin is IEnvironmentListener)
                    ((IEnvironmentListener)plugin).EnvironmentOpening();

            IsRunning = true;
            try
            {
                scheduler.Run();
            }
            catch (Exception ex)
            {
                Logger.Log(Now, LogLevel.ERROR, "Main thread exception", ex: ex);
            }
            IsRunning = false;

            foreach (IPlugin plugin in plugins)
                if (plugin is IEnvironmentListener)
                    ((IEnvironmentListener)plugin).EnvironmentClosing();
        }
    }
}
