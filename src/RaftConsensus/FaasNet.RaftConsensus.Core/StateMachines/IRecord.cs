using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public interface IRecord
    {
        void Serialize(WriteBufferContext context);
        void Deserialize(ReadBufferContext context);
    }
}
