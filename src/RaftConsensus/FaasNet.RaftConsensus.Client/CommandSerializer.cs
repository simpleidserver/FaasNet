using FaasNet.Peer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            context.WriteString(cmd.GetType().Name);
            cmd.Serialize(context);
        }

        public static byte[] Serialize<T>(T cmd) where T : ICommand
        {
            var context = new WriteBufferContext();
            context.WriteString(cmd.GetType().Name);
            cmd.Serialize(context);
            return context.Buffer.ToArray();
        }

        public static ICommand Deserialize(byte[] payload, Assembly assembly)
        {
            var context = new ReadBufferContext(payload);
            return Deserialize(context, assembly);
        }

        public static ICommand Deserialize(ReadBufferContext context, Assembly assembly) 
        {
            var name = context.NextString();
            var type = assembly.GetTypes().Single(a => a.Name == name);
            var instance = Activator.CreateInstance(type);
            var result = (ICommand)instance;
            result.Deserialize(context);
            return result;
        }
    }
}
