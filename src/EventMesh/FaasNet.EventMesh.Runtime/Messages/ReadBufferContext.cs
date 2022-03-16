using FaasNet.EventMesh.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.Messages
{
    public class ReadBufferContext
    {
        public ReadBufferContext(byte[] buffer)
        {
            Buffer = new Queue<byte>(buffer);
            CurrentOffset = 0;
        }

        public Queue<byte> Buffer { get; private set; }
        public UInt16 CurrentOffset { get; private set; }

        public int NextInt()
        {
            var result = Buffer.GetInt();
            CurrentOffset += 4;
            return result;
        }

        public string NextString()
        {
            if (!Buffer.Any())
            {
                return string.Empty;
            }

            var size = Buffer.First();
            var result = Buffer.GetString();
            CurrentOffset += size;
            return result;
        }

        public byte[] NextByteArray()
        {
            var size = Buffer.First();
            CurrentOffset += size;
            var result = Buffer.GetByteArray();
            return result;
        }

        public bool NextBoolean()
        {
            var result = Buffer.GetBoolean();
            CurrentOffset += 1;
            return result;
        }
    }
}
