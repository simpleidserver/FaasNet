using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client
{
    public interface IQueryResult
    {
        void Serialize(WriteBufferContext context);
        void Deserialize(ReadBufferContext context);
    }
}
