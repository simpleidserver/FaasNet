using FaasNet.EventMesh.Client.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class WriteBufferContext
    {
        public List<byte> Buffer { get; } = new List<byte>();

        public void WriteString(string str)
        {
            Buffer.AddRange(str.ToBytes());
        }

        public void WriteInteger(int it)
        {
            Buffer.AddRange(it.ToBytes());
        }

        public void WriteBoolean(bool i)
        {
            Buffer.Add(Convert.ToByte(i));
        }

        public void WriteByteArray(byte[] b)
        {
            var result = new List<byte>();
            result.Add((byte)b.Count());
            result.AddRange(b);
            Buffer.AddRange(result);
        }

        public void WriteStringArray(IEnumerable<string> lst)
        {
            WriteInteger(lst.Count());
            foreach(var str in lst)
            {
                WriteString(str);
            }
        }
    }
}
