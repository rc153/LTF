using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Utils
{
    public class SingleValueEnumerator<T> : IEnumerator<T>
    {
        private T data;
        private bool done;

        public T Current
        {
            get { return data; }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (done)
                return false;
            done = true;
            return true;
        }

        public void Reset()
        {
            done = false;
        }

        public void Reset(T newData)
        {
            done = false;
            data = newData;
        }
    }
}
