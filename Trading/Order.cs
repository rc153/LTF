using System;
using System.Collections.Generic;

namespace Toolkit.Trading
{
    public enum OrderType : byte
    {
        LIMIT,
        MARKET,
        LIMIT_ON_CLOSE,
        MARKET_ON_CLOSE,
        LIMIT_ON_OPEN,
        MARKET_ON_OPEN,
        // PEG, HIDDEN, ICEBERG, LIQUIDITY_ONLY, ...
    }

    public enum OrderSide : byte
    {
        BUY,
        SELL
    }

    public enum OrderState : uint
    {
        Init = 9,

        Ack = 10,

        CanceledLocal = 11,
        CanceledRemote = 12,

        ModifiedLocal = 13,
        ModifiedRemote = 14,

        ExecPartial = 15,
        ExecFull = 16,

        PendingAck = 17,
        PendingCancel = 18,
        PendingModify = 19
    }


    // use this for getting then name of a property change: http://bartwullems.blogspot.kr/2012/02/c-5-caller-info-attributes.html
    public interface IOrder : INotifyPropertyChanged<IOrder>
    {
        OrderState state { get; }
        FixedPointDecimal price { get; set; }
        uint qty { get; set; }
        uint symbol { get; }
        OrderSide side { get; }
        OrderType type { get; }
        uint seq { get; }
    }

    // todo qty in mrkt, qty pending, qty sent, qty exec
    // todo implement the property change thing
    public class Order : IOrder
    {
        public event Action<IOrder, string> PropertyChanged;

        public OrderState state { get; internal set; }
        public uint symbol { get; internal set; }
        public uint seq { get; internal set; }
        public OrderSide side { get; internal set; }
        public OrderType type { get; internal set; }
        public uint qty { get; set; }
        public FixedPointDecimal price { get; set; }

        internal void RaisePropertyChanged(string prop)
        {
            PropertyChanged(this, prop);
        }
    }
}
