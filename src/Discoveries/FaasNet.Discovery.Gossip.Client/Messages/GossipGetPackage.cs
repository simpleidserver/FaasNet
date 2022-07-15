using FaasNet.Peer.Client;

namespace FaasNet.Discovery.Gossip.Client.Messages
{
    public class GossipGetPackage : GossipPackage
    {
        public override GossipPackageTypes Type => GossipPackageTypes.GET;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
