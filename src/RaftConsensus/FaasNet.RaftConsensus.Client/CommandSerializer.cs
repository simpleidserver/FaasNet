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

        public static IEnumerable<T> Deserialize<T>(byte[] payload, Assembly assm) where T : ICommand
        {
            var result = new List<T>();
            var context = new ReadBufferContext(payload);
            var length = context.NextInt();
            for (var i = 0; i < length; i++) result.Add(Deserialize<T>(context, assm));
            return result;
        }

        public static T Deserialize<T>(ReadBufferContext context, Assembly assm) where T : ICommand
        {
            var name = context.NextString();
            var type = assm.GetTypes().Single(a => a.Name == name);
            var instance = Activator.CreateInstance(type);
            var result = (T)instance;
            result.Deserialize(context);
            return result;
        }
    }
}
