using FaasNet.Peer.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Client
{
    public static class CommandSerializer
    {
        public static byte[] Serialize<T>(IEnumerable<T> arr) where T : ICommand
        {
            var context = new WriteBufferContext();
            context.WriteInteger(arr.Count());
            foreach(var cmd in arr) Serialize(cmd, context);
            return context.Buffer.ToArray();
        }

        public static void Serialize<T>(T cmd, WriteBufferContext context) where T : ICommand
        {
            context.WriteString(cmd.GetType().AssemblyQualifiedName);
            cmd.Serialize(context);
        }

        public static byte[] Serialize<T>(T cmd) where T : ICommand
        {
            var context = new WriteBufferContext();
            context.WriteString(cmd.GetType().AssemblyQualifiedName);
            cmd.Serialize(context);
            return context.Buffer.ToArray();
        }

        public static ICommand Deserialize(byte[] payload)
        {
            var context = new ReadBufferContext(payload);
            return Deserialize(context);
        }

        public static ICommand Deserialize(ReadBufferContext context) 
        {
            var type = Type.GetType(context.NextString());
            var instance = Activator.CreateInstance(type);
            var result = (ICommand)instance;
            result.Deserialize(context);
            return result;
        }
    }
}
