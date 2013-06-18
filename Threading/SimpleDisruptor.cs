using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Toolkit.Threading
{
    // 1 reader / 1 writer minimal version of the disruptor
    [StructLayout(LayoutKind.Sequential)]
    public class SimpleDisruptor<T> where T : new()
    {
        const int DEFAULT_SIZE = 1024;

        private readonly uint mask;
        private readonly T[] all;
        private CacheLinePadding nothing1;
        private uint writeSeq;
        private CacheLinePadding nothing2;
        private uint readSeq;
        private CacheLinePadding nothing3;

        public SimpleDisruptor() : this(DEFAULT_SIZE) { }

        public SimpleDisruptor(int sizePowerOfTwo)
        {
            uint size = 1u << (int)Math.Log(sizePowerOfTwo, 2);
            mask = size - 1;

            all = new T[size];
            for (uint i = 0; i < size; i++)
                all[i] = new T();
        }

        // busy wait until space is available to write
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void Write(Action<T> callback)
        {
            while (writeSeq - readSeq >= mask) { }  // the read may be cached, so we use NoOptimization
            uint newWriteSeq = writeSeq + 1;
            callback(all[newWriteSeq & mask]);
            Volatile.Write(ref writeSeq, newWriteSeq);
            // we do a volatile write so that the new seq is seen only after the callback is done, but
            // on x86 i think it's useless. also note that a write long is not atomic, so a CAS would be
            // generated if we use a long instead of an int. It doesn't matter though if it resets to 0.
        }

        // return false if there's nothing to read
        public bool Read(Action<T> callback)
        {
            if (writeSeq == readSeq) return false;
            uint newReadSeq = readSeq + 1;
            callback(all[newReadSeq & mask]);
            Volatile.Write(ref readSeq, newReadSeq);  // similar as above
            return true;
        }
    }
}