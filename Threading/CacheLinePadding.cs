using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Threading
{
    // todo read http://www.linuxjournal.com/article/8211?page=0,1
    // todo read http://bad-concurrency.blogspot.kr/search?updated-max=2012-01-08T19:44:00Z&max-results=7
    // todo read http://mechanical-sympathy.blogspot.kr/2011/07/memory-barriersfences.html

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    internal struct CacheLinePadding { }
}
