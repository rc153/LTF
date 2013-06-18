using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Utils
{
    public struct WrappedArray<T> where T : struct
    {
        private T[] data;
        private int currentIndex;

        public WrappedArray(int size)
        {
            data = new T[size];
            currentIndex = 0;
        }

        public int Length { get { return data.Length; } }

        public T this[int index]
        {
            get
            {
                int newIndex = currentIndex + index;
                if (newIndex > data.Length) newIndex -= data.Length;
                return data[newIndex];
            }
        }

        public T first { get { return data[currentIndex]; } }

        public T last { get { return currentIndex + 1 >= data.Length ? data[0] : data[currentIndex + 1]; } }

        public void Write(T item)
        {
            currentIndex++;
            if (currentIndex >= data.Length) currentIndex = 0;
            data[currentIndex] = item;
        }
    }
}
