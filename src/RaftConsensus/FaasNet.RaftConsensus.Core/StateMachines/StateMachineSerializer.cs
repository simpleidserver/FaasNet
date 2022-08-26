using FaasNet.Peer.Client;
using System;

namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public static class StateMachineSerializer
    {
        public static IStateMachine Deserialize(Type type, byte[] payload)
        {
            var readBufferContext = new ReadBufferContext(payload);
            var instance = (IStateMachine)Activator.CreateInstance(type);
            instance.Deserialize(readBufferContext);
            return instance;
        }

        public static T Deserialize<T>(byte[] payload) where T : IStateMachine
        {
            var readBufferContext = new ReadBufferContext(payload);
            var instance = (T)Activator.CreateInstance(typeof(T));
            instance.Deserialize(readBufferContext);
            return instance;
        }
    }
}
