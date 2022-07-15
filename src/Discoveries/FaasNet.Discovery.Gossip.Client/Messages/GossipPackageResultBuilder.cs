using System.Collections.Generic;

namespace FaasNet.Discovery.Gossip.Client.Messages
{
    public class GossipPackageResultBuilder
    {
        public static GossipPackage Ok()
        {
            return new GossipResultPackage();
        }

        public static GossipPackage Get(List<PeerInfo> infos)
        {
            return new GossipGetResultPackage
            {
                PeerInfos = infos
            };
        }
    }
}
