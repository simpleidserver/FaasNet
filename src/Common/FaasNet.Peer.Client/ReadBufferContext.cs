﻿using FaasNet.Peer.Client.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaasNet.Peer.Client
{
    public class ReadBufferContext
    {
        public ReadBufferContext(byte[] buffer)
        {
            Buffer = new Queue<byte>(buffer);
            CurrentOffset = 0;
        }

        public Queue<byte> Buffer { get; private set; }
        public int CurrentOffset { get; private set; }

        public int NextInt()
        {
            var result = Buffer.GetInt();
            CurrentOffset += 4;
            return result;
        }

        public long NextLong()
        {
            var result = Buffer.GetLong();
            CurrentOffset += 8;
            return result;
        }

        public string NextString()
        {
            if (!Buffer.Any()) return string.Empty;
            var result = Buffer.GetString();
            CurrentOffset += (ushort)result.Count();
            return result;
        }

        public double NextDouble()
        {
            var result = Buffer.GetDouble();
            CurrentOffset += 8;
            return result;
        }

        public byte[] NextByteArray()
        {
            var size = NextInt();
            var result = Buffer.Dequeue(size);
            return result.ToArray();
        }

        public bool NextBoolean()
        {
            var result = Buffer.GetBoolean();
            CurrentOffset += 1;
            return result;
        }

        public IEnumerable<string> NextStringArray()
        {
            var result = new List<string>();
            var size = NextInt();
            for (int i = 0; i < size; i++) result.Add(NextString());
            return result;
        }

        public TimeSpan? NextTimeSpan()
        {
            var tick = NextLong();
            if (tick == 0) return null;
            return new TimeSpan(tick);
        }
    }
}
