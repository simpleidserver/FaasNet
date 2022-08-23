namespace FaasNet.Peer.Client
{
    public interface IPartitionedPeerClient : IPeerClient
    {
        PartitionedPeerClientTypes ClientType { get; set; }
        string PartitionKey { get; set; }
    }
}
