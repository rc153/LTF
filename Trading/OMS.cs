using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Toolkit.Configuration;
using Toolkit.Core;
using Toolkit.Ids;
using Toolkit.IO;
using Toolkit.Plugins;
using Toolkit.Utils;

namespace Toolkit.Trading
{
    public class OMS : IPlugin, IDisposable
    {
        private enum MsgType : uint
        {
            Send = 0,
            Modify = 1,
            Cancel = 2,
        }

        private static int MTU = (int)(.9 * Utils.MTU.GetFromInterface());

        private uint seqNumber;
        private bool isBufferingMode;
        private Queue<Order> recycler = new Queue<Order>();
        private OrderBook activeOrders = new OrderBook();

        private IEnvironment env;
        private IIdService idService;
        private SymbolType symbolType;
        private Dictionary<string, uint> symbolMap = new Dictionary<string, uint>();

        private Socket client;
        private byte[] sendBuffer = new byte[4096];
        private uint sendBufferPos;
        private byte[] receiveBuffer = new byte[4096];
        private uint receiveBufferPos;
        private uint receiveBufferLen;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Socket copyOfClient = client;
                client = null;
                if (copyOfClient != null)
                    copyOfClient.Close();
            }
            client = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OMS()
        {
            Dispose(false);
        }

        public void Initialize(IEnvironment env, IConfiguration cfg)
        {
            this.env = env;

            idService = env.GetIdService();
            symbolType = SymbolType.Parse(cfg.getString("symbolType"));

            IPHostEntry ipHostInfo = Dns.GetHostEntry(cfg.getString("service"));
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, Int32.Parse(cfg.getString("port")));

            // Create a TCP/IP socket.
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, true);
            client.Connect(remoteEP);

            //     Protobuf.Encode(buffer, bufferOffset, "HELLO OMS");
            //   client.Send(buffer, 0, unchecked((int)bufferOffset), SocketFlags.None);
            //    bufferOffset = 0;

            // todo pass most of this stuff in protobuf or helper class, as it's not really part of it
            int sizeRead = client.Receive(receiveBuffer);
            int sizeReadTotal = sizeRead;
            uint sizeTotal = Varint.DecodeUInt32(receiveBuffer, ref receiveBufferPos);
            uint nbSymbols = Protobuf.DecodeUInt32(receiveBuffer, ref receiveBufferPos, 0);
            symbolMap = new Dictionary<string, uint>(unchecked((int)nbSymbols));
            for (int i = 0; i < nbSymbols; i++)
            {
                string symbol = Protobuf.DecodeString(receiveBuffer, ref receiveBufferPos, 2 * i + 1);
                uint id = Protobuf.DecodeUInt32(receiveBuffer, ref receiveBufferPos, 2 * i + 2);
                symbolMap.Add(symbol, id);

                if (receiveBufferPos > sizeRead - 128 && sizeReadTotal < sizeTotal)
                {
                    int sizeToCopy = sizeRead - unchecked((int)receiveBufferPos);
                    int sizeToReceive = receiveBuffer.Length - sizeToCopy;
                    Buffer.BlockCopy(receiveBuffer, unchecked((int)receiveBufferPos), receiveBuffer, 0, sizeToCopy);
                    sizeRead = client.Receive(receiveBuffer, sizeToCopy, sizeToReceive, SocketFlags.None);
                    sizeReadTotal += sizeRead;
                    receiveBufferPos = 0;
                }
            }
            receiveBufferPos = 0;

            if (client.Available > 0)
                throw new FormatException("Too much data in OMS header");

            client.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, receiveCallback, null);
        }

        public IReadOnlyOrderBook ActiveOrders { get { return activeOrders; } }

        public IOrder CreateOrder(Id id, OrderSide side, OrderType type = OrderType.LIMIT)
        {
            Order result;
            if (recycler.Count > 0)
                result = recycler.Dequeue();
            else
                result = new Order();

            result.state = OrderState.Init;
            result.seq = seqNumber++;
            result.symbol = symbolMap[idService.GetSymbol(id, symbolType)];
            result.qty = 0;
            result.side = side;
            result.type = type;
            return result;
        }

        public void BeginBatch()
        {
            isBufferingMode = true;
        }

        // should be about 14 byte per order or 100 orders per batch
        public void Send(IOrder order)
        {
            activeOrders.Add(order);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, (uint)MsgType.Modify, 2);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, order.seq, 4);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, order.symbol, 6);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, (uint)order.side, 8);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, (uint)order.type, 10);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, order.qty, 12);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, order.price, 14);
            if (!isBufferingMode || sendBufferPos >= MTU) Flush();
        }

        public void EndBatch()
        {
            Flush();
            isBufferingMode = false;
        }

        public void Modify(IOrder order)
        {
            Protobuf.Encode(sendBuffer, ref sendBufferPos, (uint)MsgType.Modify, 2);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, order.seq, 4);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, order.qty, 6);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, order.price, 8);
            if (!isBufferingMode || sendBufferPos >= MTU) Flush();
        }

        // should be about 4 byte per cancel or 350 cancel per batch
        public void Cancel(IOrder order)
        {
            Protobuf.Encode(sendBuffer, ref sendBufferPos, (byte)MsgType.Cancel, 2);
            Protobuf.Encode(sendBuffer, ref sendBufferPos, order.seq, 4);
            if (!isBufferingMode || sendBufferPos >= MTU) Flush();
        }

        private void Flush()
        {
            client.BeginSend(sendBuffer, 0, unchecked((int)sendBufferPos), SocketFlags.None, sendCallback, null);
            sendBufferPos = 0;
        }

        // todo how will we know to spearate our messages
        // todo read them! & add other cases
        private void readData(Time nothing)
        {
            OrderState msgType = (OrderState)Protobuf.DecodeUInt32(receiveBuffer, ref receiveBufferPos, 2);
            uint seq = Protobuf.DecodeUInt32(receiveBuffer, ref receiveBufferPos, 4);
            IOrder order = activeOrders[seq];       // we should be more tolerant to problems like feedback on an order we don't have
            switch (msgType)
            {
                case OrderState.Init:
                    break;
                case OrderState.Ack:
                    //order.
                    break;
                case OrderState.CanceledLocal:
                    break;
                case OrderState.CanceledRemote:
                    break;
                case OrderState.ModifiedLocal:
                    break;
                case OrderState.ModifiedRemote:
                    break;
                case OrderState.ExecPartial:
                    break;
                case OrderState.ExecFull:
                    break;
                case OrderState.PendingAck:
                    break;
                case OrderState.PendingCancel:
                    break;
                case OrderState.PendingModify:
                    break;
                default:
                    throw new FormatException("Unknown MsgType");
            }

            uint sizeToCopy = receiveBufferLen - receiveBufferPos;
            uint sizeToReceive = (uint)receiveBuffer.Length - sizeToCopy;
            Buffer.BlockCopy(receiveBuffer, unchecked((int)receiveBufferPos), receiveBuffer, 0, unchecked((int)sizeToCopy));
            receiveBufferLen = sizeToCopy;
            client.BeginReceive(receiveBuffer, unchecked((int)sizeToCopy), unchecked((int)sizeToReceive), SocketFlags.None, receiveCallback, null);
        }

        // executed by the io completion port thread pool
        private void sendCallback(IAsyncResult ar)
        {
            SocketError result;
            client.EndSend(ar, out result);
            if (result != SocketError.Success)
                throw new SocketException();
        }

        // executed by the io completion port thread pool
        private void receiveCallback(IAsyncResult ar)
        {
            SocketError result;
            receiveBufferLen += (uint)client.EndReceive(ar, out result);
            if (result != SocketError.Success)
                throw new SocketException();

            env.Scheduler.ScheduleAfter(Time.Zero, readData);
        }
    }
}
