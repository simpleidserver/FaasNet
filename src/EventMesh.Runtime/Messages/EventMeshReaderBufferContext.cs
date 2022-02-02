﻿using EventMesh.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.Messages
{
    public class EventMeshReaderBufferContext
    {
        public EventMeshReaderBufferContext(byte[] buffer)
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
            var size = Buffer.First();
            var result = Buffer.GetString();
            CurrentOffset += size;
            return result;
        }
    }
}
