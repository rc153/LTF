using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Toolkit.Configuration;
using Toolkit.Core;

namespace Toolkit.Plugins
{
    public class PluginLoader : IEnumerable<IPlugin>
    {
        private Dictionary<string, IPlugin> pluginsByName = new Dictionary<string, IPlugin>();

        // keep these for re-entry with LoadAll to resolve dependencies 
        private IEnvironment rootEnv;
        private IConfiguration rootCfg;

        public IPlugin Get(string pluginName)
        {
            IPlugin result = null;
            if (!pluginsByName.TryGetValue(pluginName, out result))
            {
                result = LoadAsDependency(pluginName);
                pluginsByName.Add(pluginName, result);
            }
            return result;
        }

        private IPlugin LoadAsDependency(string pluginName)
        {
            foreach (string foundPluginName in rootCfg.LocalKeys)
            {
                if (foundPluginName == pluginName)
                    return Load(rootEnv, rootCfg.SubSet(pluginName));
            }
            throw new Exception("Dependency not found: " + pluginName);
        }

        public IPlugin Load(IEnvironment env, IConfiguration cfg)
        {
            string classname = cfg.getString("classname");
            IPlugin plugin = Create(classname);
            if (plugin == null) throw new InvalidOperationException("Cannot load plugin: " + classname);
            plugin.Initialize(env, cfg);
            return plugin;
        }

        public static IPlugin Create(string classname)
        {
            // Type t = types.Type.GetType(classname, true);
            Type t = AppDomain.CurrentDomain.GetAssemblies().Select(ass => ass.GetType(classname, false)).Where(typ => typ != null).First();

            return (IPlugin)Activator.CreateInstance(t, true);
            //   return (IPlugin)(t.GetConstructor(new Type[0]).Invoke(new object[0]));
        }

        public void LoadAll(IEnvironment env, IConfiguration cfg)
        {
            rootEnv = env;
            rootCfg = cfg;
            foreach (string pluginName in cfg.LocalKeys)
            {
                IConfiguration subConf = cfg.SubSet(pluginName);
                if (subConf.getBool("mustload", true))  // plugins tagged as mustload will load only as dependency
                {
                    IPlugin plugin = Load(env, subConf);
                    pluginsByName.Add(pluginName, plugin);
                }
            }
            rootEnv = null;
            rootCfg = null;
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<IPlugin> GetEnumerator() { return pluginsByName.Values.GetEnumerator(); }
    }
}
