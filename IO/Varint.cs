using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.IO
{
    public static class Varint
    {
        public static void EncodeUInt32(byte[] buffer, ref uint offset, uint value)
        {
            while (value >= 0x80)
            {
                buffer[offset++] = ((byte)(value | 0x80));
                value >>= 7;
            }
            buffer[offset++] = ((byte)value);
        }

        public static void EncodeUInt64(byte[] buffer, ref uint offset, ulong value)
        {
            while (value >= 0x80)
            {
                buffer[offset++] = ((byte)(value | 0x80));
                value >>= 7;
            }
            buffer[offset++] = ((byte)value);
        }

        public static void EncodeInt32(byte[] buffer, ref uint offset, int value)
        {
            EncodeUInt32(buffer, ref offset, unchecked((uint)((value << 1) ^ (value >> 31))));
        }

        public static void EncodeInt64(byte[] buffer, ref uint offset, long value)
        {
            EncodeUInt64(buffer, ref offset, unchecked((ulong)((value << 1) ^ (value >> 63))));
        }



        public static int DecodeInt32(byte[] buffer, ref uint offset)
        {
            uint value = DecodeUInt32(buffer, ref offset);
            uint mask = 0 - (value & 1);
            return (int)(value >> 1 ^ mask);
        }

        public static uint DecodeUInt32(byte[] buffer, ref uint offset)
        {
            uint result = buffer[offset++];
            if (result < 0x80)
                return result;

            int bitShift = 7;
            result &= 0x7f;
            while (true)
            {
                uint nextByte = buffer[offset++];

                result |= (nextByte & 0x7f) << bitShift;
                bitShift += 7;

                if (nextByte <= 0x7F) return result;
            }
        }

        public static long DecodeInt64(byte[] buffer, ref uint offset)
        {
            ulong value = DecodeUInt64(buffer, ref offset);
            ulong mask = 0L - (value & 1);
            return (long)(value >> 1 ^ mask);
        }

        public static ulong DecodeUInt64(byte[] buffer, ref uint offset)
        {
            ulong result = buffer[offset++];
            if (result < 0x80)
                return result;

            int bitShift = 7;
            result &= 0x7f;
            while (true)
            {
                ulong nextByte = buffer[offset++];

                result |= (nextByte & 0x7f) << bitShift;
                bitShift += 7;

                if (nextByte <= 0x7F) return result;
            }
        }

    }
}
