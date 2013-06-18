using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.IO
{
    public class CompactBinaryReader : IDisposable
    {
        private Stream m_stream;
        private byte[] buffer = new byte[4096]; // as we don't know the size of the data we need in each call, we have to buffer some data (match FileStream default buffer size so it doesn't copy around)
        private uint readLen;
        private uint readPos;

        public CompactBinaryReader(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (!input.CanRead)
                throw new ArgumentException("Stream is not readable");
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

        ~CompactBinaryReader()
        {
            Dispose(false);
        }

        public byte ReadByte()
        {
            FillTheBuffer(1);
            return buffer[readPos++];
        }

        private void FillTheBuffer(uint count)
        {
            uint available = this.readLen - this.readPos;
            if (available >= count)
                return;

            if (available > 0)
            {
                Buffer.BlockCopy(this.buffer, unchecked((int)this.readPos), this.buffer, 0, unchecked((int)available));
            }

            this.readPos = 0;
            this.readLen = available + unchecked((uint)m_stream.Read(this.buffer, unchecked((int)available), this.buffer.Length - unchecked((int)available)));
            if (this.readLen == 0)
                throw new EndOfStreamException();
        }

        // todo how do we check for end of stream?
        public int ReadInt32()
        {
            FillTheBuffer(5);
            return Varint.DecodeInt32(buffer, ref readPos);
        }

        //     [CLSCompliant(false)]
        public uint ReadUInt32()
        {
            FillTheBuffer(5);
            return Varint.DecodeUInt32(buffer, ref readPos);
        }

        /// <returns>An Int64 value.</returns>
        public long ReadInt64()
        {
            FillTheBuffer(10);
            return Varint.DecodeInt64(buffer, ref readPos);
        }

        // [CLSCompliant(false)]
        public ulong ReadUInt64()
        {
            FillTheBuffer(10);
            return Varint.DecodeUInt64(buffer, ref readPos);
        }

        public FixedPointDecimal ReadFixedPointDecimal()
        {
            long raw = this.ReadInt64();
            return FixedPointDecimal.FromRaw(raw);
        }
    }
}
