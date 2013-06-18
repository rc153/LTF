using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using Toolkit.Utils;

namespace Toolkit.Threading
{
    public class SimpleDisruptorContainerTLS<T> : SimpleDisruptorContainer<T> where T : new()
    {
        [ThreadStatic]
        private static SimpleDisruptor<T> THIS = null;
      
        public SimpleDisruptor<T> Get()
        {
            SimpleDisruptor<T> result = THIS;
            if (result == null)
            {
                result = new SimpleDisruptor<T>();
                THIS = result;
                Add(result);
            }
            return result;
        }
    }

    public class SimpleDisruptorContainer<T> where T : new()
    {
        private SimpleDisruptor<T>[] disruptors = new SimpleDisruptor<T>[0];
        private Thread t;

        public SimpleDisruptorContainer(ThreadPriority priority = ThreadPriority.Normal)
        {
            t = new Thread(Run);
            t.Priority = priority;
            t.Start();
        }

        public void Add(SimpleDisruptor<T> d)
        {
            disruptors = Arrays.Add(disruptors, d);
        }

        public void Remove(SimpleDisruptor<T> d)
        {
            disruptors = Arrays.Remove(disruptors, d);
        }

        private void Run()
        {
            while (true)
            {
                bool didSomething = false;
                SimpleDisruptor<T>[] all = disruptors;
                foreach (SimpleDisruptor<T> disruptor in all)
                {
                    didSomething |= disruptor.Read(data => OnNext(data));
                }
                if (!didSomething) Thread.Yield();
            }
        }

        public event Action<T> OnNext;
    }
}