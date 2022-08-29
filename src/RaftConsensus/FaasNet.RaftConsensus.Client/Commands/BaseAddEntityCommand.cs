using FaasNet.Peer.Client;
using System;

namespace FaasNet.RaftConsensus.Client.Commands
{
    public abstract class BaseAddEntityCommand<T> : ICommand where T : IEntityRecord
    {
        public T Record { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            instance.Deserialize(context);
            Record = instance;
        }

        public void Serialize(WriteBufferContext context)
        {
            Record.Serialize(context);
        }
    }
}
