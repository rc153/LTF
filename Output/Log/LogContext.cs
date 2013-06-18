using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Toolkit.Output.Log
{
    public interface ILogContext
    {
        string ToString(string format);
    }

    public class LogContext<T1> : ILogContext
    {
        private T1 item1;

        public void Populate(T1 data1)
        {
            this.item1 = data1;
        }

        public string ToString(string format)
        {
            return String.Format(format, item1);
        }
    }

    public class LogContext<T1, T2> : ILogContext
    {
        private T1 item1;
        private T2 item2;

        public void Populate(T1 data1, T2 data2)
        {
            this.item1 = data1;
            this.item2 = data2;
        }

        public string ToString(string format)
        {
            return String.Format(format, item1, item2);
        }
    }

    public class LogContext<T1, T2, T3> : ILogContext
    {
        private T1 item1;
        private T2 item2;
        private T3 item3;

        public void Populate(T1 data1, T2 data2, T3 data3)
        {
            this.item1 = data1;
            this.item2 = data2;
            this.item3 = data3;
        }

        public string ToString(string format)
        {
            return String.Format(format, item1, item2, item3);
        }
    }

    public class LogContext<T1, T2, T3, T4> : ILogContext
    {
        private T1 item1;
        private T2 item2;
        private T3 item3;
        private T4 item4;

        public void Populate(T1 data1, T2 data2, T3 data3, T4 data4)
        {
            this.item1 = data1;
            this.item2 = data2;
            this.item3 = data3;
            this.item4 = data4;
        }

        public string ToString(string format)
        {
            return String.Format(format, item1, item2, item3, item4);
        }
    }
}