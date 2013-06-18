using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Trading
{
    public interface IReadOnlyOrderBook : IEnumerable<IOrder>, INotifyPropertyChanged<IOrder>, INotifyCollectionChanged<IOrderBook, IOrder>
    {
        bool Contains(uint seq);
        bool Contains(IOrder order);
        int Count { get; }
        bool TryGetValue(uint seq, out IOrder order);
        IOrder this[uint seq] { get; set; }
    }

    public interface IOrderBook : IReadOnlyOrderBook
    {
        void Add(IOrder order);
        void Clear();
        bool Remove(IOrder order);
        bool Remove(uint seq);
    }

    public class OrderBook : IOrderBook
    {
        private Dictionary<uint, IOrder> orders = new Dictionary<uint, IOrder>();

        public event Action<IOrder, string> PropertyChanged;
        public event Action<IOrderBook, IOrder, NotifyCollectionChangedAction> CollectionChanged;

        public bool Contains(uint seq)
        {
            return orders.ContainsKey(seq);
        }

        public bool Contains(IOrder order)
        {
            return orders.ContainsValue(order);
        }

        public int Count
        {
            get { return orders.Count; }
        }

        public bool TryGetValue(uint seq, out IOrder order)
        {
            return orders.TryGetValue(seq, out order);
        }

        public IOrder this[uint seq] { get { return orders[seq]; } set { orders[seq] = value; } }

        public void Add(IOrder order)
        {
            orders.Add(order.seq, order);
            order.PropertyChanged += order_PropertyChanged;
            CollectionChanged(this, order, NotifyCollectionChangedAction.Add);
        }

        public void Clear()
        {
            foreach (IOrder order in orders.Values)
                order.PropertyChanged -= order_PropertyChanged;
            orders.Clear();
            CollectionChanged(this, null, NotifyCollectionChangedAction.Reset);
        }

        public bool Remove(IOrder order)
        {
            if (orders.Remove(order.seq))
            {
                order.PropertyChanged -= order_PropertyChanged;
                CollectionChanged(this, order, NotifyCollectionChangedAction.Remove);
                return true;
            }
            return false;
        }

        public bool Remove(uint seq)
        {
            IOrder order = null;
            if (TryGetValue(seq, out  order))
            {
                return Remove(order);
            }
            return false;
        }

        public IEnumerator<IOrder> GetEnumerator()
        {
            return orders.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void order_PropertyChanged(IOrder order, string propertyName)
        {
            PropertyChanged(order, propertyName);
        }
    }
}
