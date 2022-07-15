using FaasNet.Peer.Client;

namespace FaasNet.Discovery.Gossip.Client.Messages
{
    public class GossipResultPackage : GossipPackage
    {
        public override GossipPackageTypes Type => GossipPackageTypes.RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
