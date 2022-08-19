using FaasNet.Peer;

namespace FaasNet.Partition
{
    public interface IPartitionPeerFactory
    {
        IPeerHost Build(int port, string partitionKey);
    }
}
