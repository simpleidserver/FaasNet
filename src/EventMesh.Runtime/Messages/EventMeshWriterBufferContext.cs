using EventMesh.Runtime.Extensions;
using System.Collections.Generic;

namespace EventMesh.Runtime.Messages
{
    public class EventMeshWriterBufferContext
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
    }
}
