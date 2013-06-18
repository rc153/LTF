using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Output.Log;

namespace Toolkit.Core.Simulation
{
    static class SimulationRunner
    {
        static void Main(string[] args)
        {
            SimulationEnvironment env = new SimulationEnvironment();
            FileConfiguration cfg = new FileConfiguration(args[0]);
            env.Run(cfg);
            Logger.Shutdown();
        }
    }
}
