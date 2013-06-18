using Disruptor;
using Disruptor.Dsl;
using Disruptor.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Threading;

// namespace must match filename for the little GetRootLength trick to work
namespace Toolkit.Output.Log
{

    public enum LogLevel : byte
    {
        DEBUG = MsgType.LOG + 0,
        INFO = MsgType.LOG + 1,
        WARNING = MsgType.LOG + 2,
        ERROR = MsgType.LOG + 3,
    }


    // check only global level in here, as it's the main use case
    public static class Logger
    {
        private const int SHUTDOWN_LEVEL = 9;
        private /*readonly*/ static int maxLevel;
        private readonly static Dictionary<string, LogLevel> loggerLevels = new Dictionary<string, LogLevel>();
        internal readonly static int rootLength;

        static Logger()
        {
            FileConfiguration cfg = new FileConfiguration("./logger.config");
            IConfiguration cfg2 = cfg.SubSet("level");
            maxLevel = (byte)cfg2.getEnum("default", LogLevel.WARNING);
            foreach (string key in cfg2.LocalKeys)
            {
                loggerLevels.Add(key, cfg2.getEnum(key, LogLevel.WARNING));
            }
            rootLength = GetRootLength();

            // todo load a given output module
            // todo load a given output writer
        }

        // get the length of the path to remove at the begining
        private static int GetRootLength([CallerFilePath] string sourceFilePath = "")
        {
            string thisShortPath = typeof(Logger).FullName.Replace('.', '\\') + ".cs";
            return sourceFilePath.IndexOf(thisShortPath);
        }

        // use this to flush and stop all logging
        public static void Shutdown()
        {
            maxLevel = SHUTDOWN_LEVEL;
        }

        internal static bool IsEnabledFor(string logger, LogLevel level)
        {
            LogLevel loggerMaxLevel;
            if (loggerLevels.TryGetValue(logger, out loggerMaxLevel))
            {
                return (loggerMaxLevel <= level);
            }
            return true;
        }

        private static bool IsEnabledFor(LogLevel level)
        {
            return (maxLevel <= (byte)level);
        }

        public static void Log(ulong now, LogLevel level, string format, Exception ex = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (IsEnabledFor(level))
            {
                Output.DoLog(now, level, format, null, ex, sourceFilePath, sourceLineNumber);
            }
        }

        public static void Log<T1>(ulong now, LogLevel level, string format, T1 item1, Exception ex = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (IsEnabledFor(level))
            {
                IRecyclerDynamicElement<LogContext<T1>> context = RecyclerDynamicTLS<LogContext<T1>>.Get().Acquire();
                context.Value.Populate(item1);
                Output.DoLog(now, level, format, context, ex, sourceFilePath, sourceLineNumber);
            }
        }

        public static void Log<T1, T2>(ulong now, LogLevel level, string format, T1 item1, T2 item2, Exception ex = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (IsEnabledFor(level))
            {
                IRecyclerDynamicElement<LogContext<T1, T2>> context = RecyclerDynamicTLS<LogContext<T1, T2>>.Get().Acquire();
                context.Value.Populate(item1, item2);
                Output.DoLog(now, level, format, context, ex, sourceFilePath, sourceLineNumber);
            }
        }

        public static void Log<T1, T2, T3>(ulong now, LogLevel level, string format, T1 item1, T2 item2, T3 item3, Exception ex = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (IsEnabledFor(level))
            {
                IRecyclerDynamicElement<LogContext<T1, T2, T3>> context = RecyclerDynamicTLS<LogContext<T1, T2, T3>>.Get().Acquire();
                context.Value.Populate(item1, item2, item3);
                Output.DoLog(now, level, format, context, ex, sourceFilePath, sourceLineNumber);
            }
        }

        public static void Log<T1, T2, T3, T4>(ulong now, LogLevel level, string format, T1 item1, T2 item2, T3 item3, T4 item4, Exception ex = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (IsEnabledFor(level))
            {
                IRecyclerDynamicElement<LogContext<T1, T2, T3, T4>> context = RecyclerDynamicTLS<LogContext<T1, T2, T3, T4>>.Get().Acquire();
                context.Value.Populate(item1, item2, item3, item4);
                Output.DoLog(now, level, format, context, ex, sourceFilePath, sourceLineNumber);
            }
        }
    }
}
