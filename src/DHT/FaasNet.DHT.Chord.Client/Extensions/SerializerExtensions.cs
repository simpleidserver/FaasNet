using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaasNet.DHT.Chord.Client.Extensions
{
    public static class SerializerExtensions
    {
        #region Serialize

        public static ICollection<byte> ToBytes(this string str)
        {
            var result = new List<byte>();
            str = str ?? string.Empty;
            var payload = Encoding.ASCII.GetBytes(str);
            result.AddRange(payload.Count().ToBytes());
            result.AddRange(payload);
            return result;
        }

        public static ICollection<byte> ToBytes(this int it)
        {
            var result = new List<byte>
            {
                (byte)(it >> 24),
                (byte)((it >> 16) & 0xFF),
                (byte)((it >> 8) & 0xFF),
                (byte)(it & 0xFF)
            };
            return result;
        }

        public static ICollection<byte> ToBytes(this long l)
        {
            var result = new List<byte>
            {
               (byte) l,
               (byte) (l >> 8),
               (byte) (l >> 16),
               (byte) (l >> 24),
               (byte) (l >> 32),
               (byte) (l >> 40),
               (byte) (l >> 48),
               (byte) (l >> 56)
            };
            return result;
        }

        #endregion

        #region Deserialize

        public static int GetInt(this Queue<byte> queue)
        {
            var payload = queue.Dequeue(4);
            var result = ((payload.ElementAt(0) & 0xFF) << 24) | ((payload.ElementAt(1) & 0xFF) << 16) | ((payload.ElementAt(2) & 0xFF) << 8) | (0xFF & payload.ElementAt(3));
            return result;
        }

        public static long GetLong(this Queue<byte> queue)
        {
            var payload = queue.Dequeue(8);
            var result = ((long)payload.ElementAt(7) << 56)
                   | ((long)payload.ElementAt(6) & 0xff) << 48
                   | ((long)payload.ElementAt(5) & 0xff) << 40
                   | ((long)payload.ElementAt(4) & 0xff) << 32
                   | ((long)payload.ElementAt(3) & 0xff) << 24
                   | ((long)payload.ElementAt(2) & 0xff) << 16
                   | ((long)payload.ElementAt(1) & 0xff) << 8
                   | ((long)payload.ElementAt(0) & 0xff);
            return result;
        }

        public static string GetString(this Queue<byte> queue)
        {
            var size = (uint)queue.GetInt();
            return Encoding.ASCII.GetString(queue.Dequeue(size).ToArray());
        }

        public static byte[] GetByteArray(this Queue<byte> queue)
        {
            var size = queue.Dequeue();
            return queue.Dequeue(size).ToArray();
        }

        public static bool GetBoolean(this Queue<byte> queue)
        {
            var payload = queue.Dequeue(1);
            return Convert.ToBoolean(payload.ElementAt(0));
        }

        public static IEnumerable<byte> Dequeue(this Queue<byte> queue, uint number)
        {
            var result = new List<byte>();
            for (var i = 0; i < number; i++)
            {
                result.Add(queue.Dequeue());
            }

            return result.ToArray();
        }

        #endregion
    }
}
