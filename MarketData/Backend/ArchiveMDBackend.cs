using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Configuration;
using SharpCompress.Archive;
using Toolkit.IO;
using System.IO;
using Toolkit.Plugins;
using Toolkit.Core;

namespace Toolkit.MarketData.Backend
{
    public class ArchiveMDBackend : IMDBackend, IPlugin
    {
        // private IArchive theArchive;
        private Dictionary<string, IArchiveEntry> theFiles = new Dictionary<string, IArchiveEntry>();

        public void Initialize(IEnvironment env, IConfiguration cfg)
        {
            IArchive theArchive = ArchiveFactory.Open(File.Open(cfg.getString("source"), FileMode.Open, FileAccess.Read, FileShare.Read));
            foreach (IArchiveEntry entry in theArchive.Entries)
            {
                theFiles.Add(entry.FilePath, entry);
            }
        }

        public CompactBinaryReader getReader(string symbol)
        {
            return new CompactBinaryReader(theFiles[symbol].OpenEntryStream());
        }
    }
}
