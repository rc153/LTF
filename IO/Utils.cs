using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolkit.Utils;
using System.Threading.Tasks;

namespace Toolkit.IO
{
    public static class Utils
    {
        public static Tuple<string[], IEnumerable<string[]>> ReadCSV(string filename)
        {
            var file = File.ReadLines(filename);

            var query1 = (from line in file.Take(1)
                          let headers = line.Split(',').TrimInPlace()
                          select headers.Skip(1).ToArray()).First();

            var query2 = from line in file.Skip(1)
                         let data = line.Split(',').TrimInPlace()
                         select data.ToArray();

            return new Tuple<string[], IEnumerable<string[]>>(query1, query2);
        }
    }
}
