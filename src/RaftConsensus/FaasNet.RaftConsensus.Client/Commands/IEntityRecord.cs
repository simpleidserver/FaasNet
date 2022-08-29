using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Commands
{
    public interface IEntityRecord
    {
        string Id { get; set; }
        void Serialize(WriteBufferContext context);
        void Deserialize(ReadBufferContext context);
    }
}
