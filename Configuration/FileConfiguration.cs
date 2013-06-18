using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Ids;
using Toolkit.Utils;

namespace Toolkit.Configuration
{
    public class FileConfiguration : BaseConfiguration
    {
        public FileConfiguration(string filename) : base(Read(filename)) { }

        private static IEnumerable<string[]> Read(string filename)
        {
            var file = File.ReadLines(filename);

            /*var includes = from line in file
                           where line.StartsWith("#include ")
                           select File.ReadLines(line.Substring(9));

            return from line in includes.Aggregate((a, b) => a.Concat(b)).Concat(file)
                   let items = line.Split('=')
                   select items;*/

            return from line in ParseIncludes(file)
                   where !String.IsNullOrWhiteSpace(line)
                   where !line.StartsWith("#")
                   select line.Split('=').TrimInPlace();
        }

        private static IEnumerable<string> ParseIncludes(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                if (line.StartsWith("#include "))
                {
                    foreach (string lineInc in File.ReadLines(line.Substring(9)))
                    {
                        yield return lineInc;
                    }
                }
                yield return line;
            }
        }
    }
}
