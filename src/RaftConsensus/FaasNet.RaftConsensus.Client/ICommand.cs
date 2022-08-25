using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client
{
    public interface ICommand
    {
        void Serialize(WriteBufferContext context);
        void Deserialize(ReadBufferContext context);
    }
}
