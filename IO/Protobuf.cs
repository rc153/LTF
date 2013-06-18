using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.IO
{
    // a minimal implementation of google's protocol buffers
    // https://developers.google.com/protocol-buffers/docs/encoding

    // todo write tests
    public static class Protobuf
    {

        private static void EncodeFieldAndType(byte[] buffer, ref uint offset, byte wire_type, uint field_number)
        {
            Varint.EncodeUInt32(buffer, ref offset, (field_number << 3) | wire_type);
        }

        public static void Encode(byte[] buffer, ref uint offset, bool value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 0, field_number);
            buffer[offset++] = (byte)(value ? 1 : 0);
        }

        public static void Encode(byte[] buffer, ref uint offset, int value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 0, field_number);
            Varint.EncodeInt32(buffer, ref offset, value);
        }

        public static void Encode(byte[] buffer, ref uint offset, uint value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 0, field_number);
            Varint.EncodeUInt32(buffer, ref offset, value);
        }

        public static void Encode(byte[] buffer, ref uint offset, long value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 0, field_number);
            Varint.EncodeInt64(buffer, ref offset, value);
        }

        public static void Encode(byte[] buffer, ref uint offset, ulong value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 0, field_number);
            Varint.EncodeUInt64(buffer, ref offset, value);
        }

        public static void Encode(byte[] buffer, ref uint offset, FixedPointDecimal value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 0, field_number);
            Varint.EncodeInt64(buffer, ref offset, value.ToRaw());
        }

        public static void Encode(byte[] buffer, ref uint offset, double value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 1, field_number);
            byte[] bits = BitConverter.GetBytes(value);
            buffer[offset++] = bits[0]; buffer[offset++] = bits[1];
            buffer[offset++] = bits[2]; buffer[offset++] = bits[3];
            buffer[offset++] = bits[4]; buffer[offset++] = bits[5];
            buffer[offset++] = bits[6]; buffer[offset++] = bits[7];
        }

        public static void Encode(byte[] buffer, ref uint offset, string value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 2, field_number);
            byte[] bits = Encoding.UTF8.GetBytes(value);
            Varint.EncodeUInt32(buffer, ref offset, (uint)bits.Length);
            Buffer.BlockCopy(bits, 0, buffer, unchecked((int)offset), bits.Length);
            offset += unchecked((uint)bits.Length);
        }

        public static void Encode(byte[] buffer, ref uint offset, byte[] value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 2, field_number);
            byte[] bits = value;
            Varint.EncodeUInt32(buffer, ref offset, (uint)bits.Length);
            Buffer.BlockCopy(bits, 0, buffer, unchecked((int)offset), bits.Length);
            offset += unchecked((uint)bits.Length);
        }

        public static void Encode(byte[] buffer, ref uint offset, float value, byte field_number)
        {
            EncodeFieldAndType(buffer, ref offset, 5, field_number);
            byte[] bits = BitConverter.GetBytes((float)(object)value);
            buffer[offset++] = bits[0]; buffer[offset++] = bits[1];
            buffer[offset++] = bits[2]; buffer[offset++] = bits[3];
        }



        public static int DecodeInt32(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 0);
            return Varint.DecodeInt32(buffer, ref offset);
        }

        public static uint DecodeUInt32(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 0);
            return Varint.DecodeUInt32(buffer, ref offset);
        }

        public static long DecodeInt64(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 0);
            return Varint.DecodeInt64(buffer, ref offset);
        }

        public static ulong DecodeUInt64(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 0);
            return Varint.DecodeUInt64(buffer, ref offset);
        }

        public static bool DecodeBool(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 0);
            return buffer[offset++] > 0;
        }

        public static FixedPointDecimal DecodeFixedPointDecimal(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 0);
            return FixedPointDecimal.FromRaw(Varint.DecodeInt64(buffer, ref offset));
        }

        public static double DecodeDouble(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 1);
            double value = BitConverter.ToDouble(buffer, unchecked((int)offset));
            offset += 8;
            return value;
        }

        public static string DecodeString(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 2);
            uint length = Varint.DecodeUInt32(buffer, ref offset);
            string value = Encoding.UTF8.GetString(buffer, unchecked((int)offset), unchecked((int)length));
            offset += length;
            return value;
        }

        public static byte[] DecodeByteArray(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 2);
            uint length = Varint.DecodeUInt32(buffer, ref offset);
            byte[] value = new byte[length];
            Buffer.BlockCopy(buffer, unchecked((int)offset), value, 0, unchecked((int)length));
            offset += length;
            return value;
        }

        public static float DecodeFloat(byte[] buffer, ref uint offset, int fieldNumber)
        {
            SkipToFieldNumberAndCheckWireType(buffer, ref offset, fieldNumber, 5);
            float value = BitConverter.ToSingle(buffer, unchecked((int)offset));
            offset += 4;
            return value;
        }

        public static bool HasField(byte[] buffer, ref uint offset, int fieldNumber)
        {
            uint localOffset = offset;
            return SkipToFieldNumberAndCheckWireType(buffer, ref  localOffset, fieldNumber, -1, false);
        }

        private static bool SkipToFieldNumberAndCheckWireType(byte[] buffer, ref uint offset, int fieldNumber, int wireType, bool throwOnNotFound = true)
        {
            uint raw = Varint.DecodeUInt32(buffer, ref offset);
            uint currentWireType = raw & 0x7;
            uint currentFieldNumber = raw >> 3;
            while (fieldNumber > currentFieldNumber)
            {
                switch (currentWireType)
                {
                    case 0:
                        Varint.DecodeUInt64(buffer, ref offset);
                        break;
                    case 1:
                        offset += 8;
                        break;
                    case 2:
                        uint length = Varint.DecodeUInt32(buffer, ref offset);
                        offset += length;
                        break;
                    case 5:
                        offset += 4;
                        break;
                    default:
                        throw new FormatException("wireType not supported: " + currentWireType);
                }
                raw = Varint.DecodeUInt32(buffer, ref offset);
                currentWireType = raw & 0x7;
                currentFieldNumber = raw >> 3;
            }

            if (fieldNumber == currentFieldNumber)
            {
                if (wireType > 0 && currentWireType != wireType)
                    throw new FormatException("unexpected wireType: should be " + wireType + " is " + currentWireType);
                return true;
            }

            if (throwOnNotFound)
                throw new FormatException("field not found " + fieldNumber + " before " + currentFieldNumber);
            return false;
        }
    }
}
