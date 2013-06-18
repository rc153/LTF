using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Core;
using Toolkit.Output.Log;
using Toolkit.Threading;

namespace Toolkit.Output
{
    // higher 4 bits are flags for type, lower 4 bits are subtype
    internal enum MsgType : byte
    {
        LOG = 1 << 4, /* 0 ~ 15 */
        GUI = 1 << 5, /* 32 ~ 47 */
        PERF = 1 << 6, /* 64 ~ 79 */
        RES = 1 << 7, /* 128 ~ 143 */
    }

    internal sealed class OutputPayload
    {
        public ulong time;
        public MsgType type;
        public string subject;

        /* LOG */
        public string format;
        public IRecyclerDynamicElement<ILogContext> context;
        public Exception ex;

        /* PERF */
        public uint hits;
        public Time elapsed;

        /* GUI */

    }
}
