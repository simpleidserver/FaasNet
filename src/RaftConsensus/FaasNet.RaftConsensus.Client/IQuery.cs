using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client
{
    public interface IQuery
    {
        void Serialize(WriteBufferContext context);
        void Deserialize(ReadBufferContext context);
    }
}
