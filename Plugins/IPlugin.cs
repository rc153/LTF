using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Core;

namespace Toolkit.Plugins
{
    public interface IPlugin
    {
        void Initialize(IEnvironment env, IConfiguration cfg);
    }
}
