using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.IO;
using System.IO;
using Toolkit.Plugins;
using Toolkit.Core;

namespace Toolkit.MarketData.Backend
{
    public class DirectoryMDBackend : IMDBackend, IPlugin
    {
        private Dictionary<string, FileInfo> theFiles = new Dictionary<string, FileInfo>();

        public void Initialize(IEnvironment env, IConfiguration cfg)
        {
            string dirPath = cfg.getString("source");
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            foreach (FileInfo file in dir.EnumerateFiles())
            {
                theFiles.Add(file.Name, file);
            }
        }

        public CompactBinaryReader getReader(string symbol)
        {
            return new CompactBinaryReader(theFiles[symbol].Open(FileMode.Open, FileAccess.Read, FileShare.Read));
        }
    }
}
