using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Toolkit.Threading
{
    public class RecyclerDynamicTLS<T> where T : new()
    {
        [ThreadStatic]
        private static RecyclerDynamic<T> THIS = null;

        public static RecyclerDynamic<T> Get()
        {
            RecyclerDynamic<T> result = THIS;
            if (result == null)
            {
                result = new RecyclerDynamic<T>();
                THIS = result;
            }
            return result;
        }
    }

    // you can call release here or on the recycler
    public interface IRecyclerDynamicElement<out T>
    {
        T Value { get; }
        void Release();
    }

    // 1 reader / 1 writer variable size recycler for out of order recycling
    public class RecyclerDynamic<T> where T : new()
    {
        const int DEFAULT_SIZE = 3;

        public class RecyclerDynamicElement : IRecyclerDynamicElement<T>
        {
            private bool isAvailable;

            internal bool IsAvailable { get { return isAvailable; } }
            internal void Acquire() { isAvailable = false; }
            public void Release() { Volatile.Write(ref isAvailable, true); }

            internal RecyclerDynamicElement Next;
            internal RecyclerDynamicElement Prev;
            public T Value { get; internal set; }

            internal RecyclerDynamicElement AddBefore(T newT)
            {
                Prev = new RecyclerDynamicElement() { Value = newT, Next = this, Prev = this.Prev };
                return Prev;
            }
        }

        private CacheLinePadding nothing1;
        private RecyclerDynamicElement head;
        private CacheLinePadding nothing2;
        private RecyclerDynamicElement currentWrite;
        private CacheLinePadding nothing3;

        public RecyclerDynamic() : this(DEFAULT_SIZE) { }

        public RecyclerDynamic(int size)
        {
            head = new RecyclerDynamicElement() { Value = new T() };
            for (int i = 0; i < size - 1; i++)
                head = head.AddBefore(new T());

            currentWrite = head;
        }

        public IRecyclerDynamicElement<T> Acquire()
        {
            if (currentWrite.IsAvailable)
            {
                RecyclerDynamicElement lc = currentWrite;
                if (currentWrite.Next != null)
                    currentWrite = currentWrite.Next;
                else
                    currentWrite = head;
                lc.Acquire();
                return lc;
            }

            currentWrite = currentWrite.AddBefore(new T());
            // lc.Acquire(); // Useless
            return currentWrite;
        }

        public void Release(IRecyclerDynamicElement<T> item)
        {
            item.Release();
        }
    }
}
