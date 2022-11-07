using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client
{
    public interface IQueryResult
    {
        void Serialize(WriteBufferContext context);
        void Deserialize(ReadBufferContext context);
    }

    public class EmptyQueryResult : IQueryResult
    {
        public void Deserialize(ReadBufferContext context)
        {
        }

        public void Serialize(WriteBufferContext context)
        {
        }
    }
}
