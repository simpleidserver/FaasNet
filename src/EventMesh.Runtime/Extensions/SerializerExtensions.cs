using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventMesh.Runtime.Extensions
{
    public static class SerializerExtensions
    {
        #region Serialize

        public static ICollection<byte> ToBytes(this string str)
        {
            var result = new List<byte>();
            str = str ?? string.Empty;
            var payload = Encoding.ASCII.GetBytes(str);
            result.Add((byte)payload.Count());
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

        #endregion

        #region Deserialize

        public static int GetInt(this Queue<byte> queue)
        {
            var payload = queue.Dequeue(4);
            var result = ((payload.ElementAt(0) & 0xFF) << 24) | ((payload.ElementAt(1) & 0xFF) << 16) | ((payload.ElementAt(2) & 0xFF) << 8) | (0xFF & payload.ElementAt(3));
            return result;
        }

        public static string GetString(this Queue<byte> queue)
        {
            var size = queue.Dequeue();
            return Encoding.ASCII.GetString(queue.Dequeue(size).ToArray());
        }

        public static byte[] GetByteArray(this Queue<byte> queue)
        {
            var size = queue.Dequeue();
            return queue.Dequeue(size).ToArray();
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
