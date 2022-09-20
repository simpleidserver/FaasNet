using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client
{
    public interface ISerializable
    {
        void Serialize(WriteBufferContext context);
        void Deserialize(ReadBufferContext context);
    }
}
