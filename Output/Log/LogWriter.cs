using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Core;

namespace Toolkit.Output.Log
{
    // todo protobuf encode writer
    // check per-logger level here
    internal static class LogWriter
    {
        public static void OnNext(OutputPayload data)
        {
            string where = data.subject.Substring(Logger.rootLength);
            if (Logger.IsEnabledFor(where, (LogLevel)data.type))
            {
                string when = HiResDateTime.Format(data.time);
                string what = ((LogLevel)data.type).ToString();
                string how = data.context == null ? data.format : data.context.Value.ToString(data.format);
                string ex = data.ex == null ? "" : data.ex.ToString();
                Console.WriteLine("{0}|{1}|{2}|{3}|{4}", when, where, what, how, ex);
            }
            data.context.Release();
        }
    }
}
