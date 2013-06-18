using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Book;
using Toolkit.Configuration;
using Toolkit.Ids;
using Toolkit.Plugins;

namespace Toolkit.Core
{
    public interface IEnvironment
    {
        IPlugin GetPlugin(string pluginName);
        IPlugin LoadPlugin(IConfiguration cfg);

        IIdService GetIdService();
        IUniverseService GetUniverseService();

        bool IsRunning { get; }
        IScheduler Scheduler { get; }
    }
}
