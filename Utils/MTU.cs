using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Utils
{
    public static class MTU
    {
        public static int GetFromInterface()
        {
            var query = from iface in NetworkInterface.GetAllNetworkInterfaces()
                        where iface.NetworkInterfaceType != NetworkInterfaceType.Loopback
                        where iface.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                        where iface.OperationalStatus == OperationalStatus.Up
                        select iface.GetIPProperties().GetIPv4Properties().Mtu;

            int[] mtus = query.ToArray();
            if (mtus.Length > 1 || mtus.Length <= 0)
                throw new InvalidOperationException();

            return mtus[0];
        }

        public static int GetFromTrial(string hostname)
        {
            UdpClient s = new UdpClient();
            s.DontFragment = true;

            int lo = 1;
            int hi = 3000;

            // try to find an upper bound
            while (trySendUdp(s, hostname, hi))
            {
                hi <<= 1;
            }

            // binary search
            while (lo != hi)
            {
                int mid = (lo + hi) >> 1;
                if (trySendUdp(s, hostname, mid)) lo = mid + 1;
                else hi = mid - 1;
            }

            // final test
            if (!trySendUdp(s, hostname, lo)) lo--;
            s.Close();
            return lo + 28; // add ip + udp header sizes
        }

        private static bool trySendUdp(UdpClient s, string hostname, int size)
        {
            byte[] b = new byte[size];
            try { s.Send(b, size, hostname, 1234); }
            catch (SocketException) { return false; }
            return true;
        }
    }
}
