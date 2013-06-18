using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.IO
{
    public class CompactBinaryWriter : IDisposable
    {
        private Stream m_stream;
        private byte[] buffer = new byte[10];

        public CompactBinaryWriter(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (!input.CanWrite)
                throw new ArgumentException("Stream is not writeable");
            m_stream = input;
        }

        public virtual void Close()
        {
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stream copyOfStream = m_stream;
                m_stream = null;
                if (copyOfStream != null)
                    copyOfStream.Close();
            }
            m_stream = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CompactBinaryWriter()
        {
            Dispose(false);
        }

        public void WriteByte(byte b)
        {
            m_stream.WriteByte(b);
        }

        public void WriteUInt64(ulong value)
        {
            uint offset = 0;
            Varint.EncodeUInt64(buffer, ref offset, value);
            m_stream.Write(buffer, 0, unchecked((int)offset));
        }

        public void WriteInt64(long value)
        {
            uint offset = 0;
            Varint.EncodeInt64(buffer, ref offset, value);
            m_stream.Write(buffer, 0, unchecked((int)offset));
        }

        public void WriteUInt32(uint value)
        {
            uint offset = 0;
            Varint.EncodeUInt32(buffer, ref offset, value);
            m_stream.Write(buffer, 0, unchecked((int)offset));
        }

        public void WriteInt32(int value)
        {
            uint offset = 0;
            Varint.EncodeInt32(buffer, ref offset, value);
            m_stream.Write(buffer, 0, unchecked((int)offset));
        }

        public void WriteFixedPointDecimal(FixedPointDecimal fp)
        {
            WriteInt64(fp.ToRaw());
        }
    }
}
