using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    /// <summary>
    /// Поток основывается на Мемори стриме, но не закрывается итд
    /// </summary>
    public class MemoryBasedStream : Stream
    {
        MemoryStream baseStream = null;

        public MemoryBasedStream()
        {
            baseStream = new MemoryStream();
        }
        public MemoryBasedStream(byte[] def)
        {
            baseStream = new MemoryStream(def);
            Position = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="def">Искодный поток, читает из потока (текущего положения) до конца</param>
        /// <param name="setPositionIn0">Позиция после прочтения из базового потока в 0 (true), в конец (false)</param>
        public MemoryBasedStream(Stream def, bool setPositionIn0 = true)
        {
            baseStream = new MemoryStream();
            baseStream.ReadFromStream(def);
            if (setPositionIn0)
            {
                baseStream.Position = 0;
            }
        }

        public override bool CanRead
        {
            get { return baseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return baseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return baseStream.CanWrite; }
        }

        public override void Flush()
        {
            // не флашим
        }

        public override long Length
        {
            get
            {
                return baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return baseStream.Position;
            }
            set
            {
                baseStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        ~MemoryBasedStream()
        {
            try
            {
                if (baseStream != null)
                {
                    baseStream.Dispose();
                }
            }
            catch
            {
                // просто гасим ошибку
            }
            finally
            { baseStream = null; }
        }

        public byte[] ToArray()
        {
            var res = baseStream.ToArray();
            Position = 0;
            return res;

        }
    }
}

