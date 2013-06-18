using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Toolkit.Threading
{
    public class RecyclerDisruptorTLS<T> where T : new()
    {
        [ThreadStatic]
        private static RecyclerDisruptor<T> THIS = null;

        public static RecyclerDisruptor<T> Get()
        {
            RecyclerDisruptor<T> result = THIS;
            if (result == null)
            {
                result = new RecyclerDisruptor<T>();
                THIS = result;
            }
            return result;
        }
    }

    // 1 reader / 1 writer fixed size recycler for same order recycling
    [StructLayout(LayoutKind.Sequential)]
    public class RecyclerDisruptor<T> where T : new()
    {
        const int DEFAULT_SIZE = 1024;

        private readonly ulong mask;
        private readonly T[] all;
        private CacheLinePadding nothing1;
        private ulong writeSeq;
        private CacheLinePadding nothing2;
        private ulong readSeq;
        private CacheLinePadding nothing3;

        public RecyclerDisruptor() : this(DEFAULT_SIZE) { }

        public RecyclerDisruptor(int sizePowerOfTwo)
        {
            ulong size = 1ul << (int)Math.Log(sizePowerOfTwo, 2);
            mask = size - 1;

            all = new T[size];
            for (ulong i = 0; i < size; i++)
                all[i] = new T();
        }

        // busy wait until an item is available
        public T Acquire()
        {
            while (writeSeq - readSeq >= mask) { }
            writeSeq++;
            return all[writeSeq & mask];
        }

        // release one item, you should not release something you didn't acquire as we don't check that
        public void Release()
        {
            Volatile.Write(ref readSeq, readSeq + 1);
        }
    }
}
