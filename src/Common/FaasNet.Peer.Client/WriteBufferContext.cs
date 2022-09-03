using FaasNet.Peer.Client.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Peer.Client
{
    public class WriteBufferContext
    {
        public List<byte> Buffer { get; } = new List<byte>();

        public WriteBufferContext WriteString(string str)
        {
            Buffer.AddRange(str.ToBytes());
            return this;
        }

        public WriteBufferContext WriteInteger(int it)
        {
            Buffer.AddRange(it.ToBytes());
            return this;
        }

        public WriteBufferContext WriteLong(long it)
        {
            Buffer.AddRange(it.ToBytes());
            return this;
        }

        public WriteBufferContext WriteBoolean(bool i)
        {
            Buffer.Add(Convert.ToByte(i));
            return this;
        }

        public WriteBufferContext WriteByteArray(byte[] b)
        {
            var result = new List<byte>();
            var nb = b == null ? 0 : b.Length;
            result.AddRange(nb.ToBytes());
            if (nb > 0) result.AddRange(b);
            Buffer.AddRange(result);
            return this;
        }

        public WriteBufferContext WriteStringArray(IEnumerable<string> lst)
        {
            WriteInteger(lst.Count());
            foreach (var str in lst) WriteString(str);
            return this;
        }

        public WriteBufferContext WriteTimeSpan(TimeSpan? sp)
        {
            if (sp == null)
            {
                WriteLong(0);
            }

            WriteLong(sp.Value.Ticks);
            return this;
        }
    }
}
