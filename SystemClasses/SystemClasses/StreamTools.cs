using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
    /// <summary>
    /// Тулы для работы с потоками (данных)
    /// </summary>
    public static class StreamTools
    {
        /// <summary>
        /// Прочитать из потока
        /// </summary>
        /// <param name="S">Поток</param>
        /// <param name="bufferLenght">Длинна буфера</param>
        public static T ReadFromStream<T>(this T baseStream, Stream s, int bufferLenght = 128 * 1024)
            where T : Stream
        {
            byte[] Buffer = new byte[bufferLenght];
            int RLen = s.Read(Buffer, 0, Buffer.Length);
            while (RLen > 0)
            {
                baseStream.Write(Buffer, 0, RLen);
                RLen = s.Read(Buffer, 0, Buffer.Length);
            }
            return baseStream;
        }

        /// <summary>
        /// Прочитать из потока
        /// </summary>
        /// <param name="S">Поток</param>
        /// <param name="ByteLenght">Длинна буфера</param>
        /// <param name="readForceCount">Принудительное колво попыток чтения (пауза 30 мс)</param>
        public static T ReadFromStream<T>(this T baseStream, Stream s, long ReadLength, int bufferLenght = 128 * 1024, int readForceCount = 100, bool throwEx = true, TimeSpan? wait_new_data = null)
            where T : Stream
        {
            byte[] Buffer = new byte[bufferLenght];
            int RLen = s.Read(Buffer, 0, Buffer.Length);
            int readCount = 0;
            while (((RLen > 0) || (readForceCount > 0 && readCount < readForceCount)) && (baseStream.Length < ReadLength))
            {
                if (RLen > 0)
                {
                    baseStream.Write(Buffer, 0, RLen);
                    readForceCount = 0;
                }
                else
                {
                    readForceCount++;
                    Thread.Sleep(wait_new_data ?? new TimeSpan(0, 0, 0, 0, 30));
                }
                RLen = s.Read(Buffer, 0, Buffer.Length);
            }

            if (readForceCount > 0 && baseStream.Length < ReadLength && throwEx)
            {
                throw new IndexOutOfRangeException("поток не прочтён до конца");
            }
            return baseStream;
        }

    }
}

