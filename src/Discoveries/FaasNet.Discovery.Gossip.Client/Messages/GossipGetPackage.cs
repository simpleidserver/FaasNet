using FaasNet.Peer.Client;

namespace FaasNet.Discovery.Gossip.Client.Messages
{
    public class GossipGetPackage : GossipPackage
    {
        public override GossipPackageTypes Type => GossipPackageTypes.GET;

        public string PartitionKey { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(PartitionKey);
        }

        public void Extract(ReadBufferContext context)
        {
            PartitionKey = context.NextString();
        }
    }
}
