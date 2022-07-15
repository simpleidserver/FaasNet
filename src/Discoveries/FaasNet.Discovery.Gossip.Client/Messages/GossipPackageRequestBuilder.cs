using System.Collections.Generic;

namespace FaasNet.Discovery.Gossip.Client.Messages
{
    public static class GossipPackageRequestBuilder
    {
        public static GossipPackage Sync(ICollection<PeerInfo> peerInfos)
        {
            return new GossipSyncPackage
            {
                PeerInfos = peerInfos
            };
        }

        public static GossipPackage Get()
        {
            return new GossipGetPackage();
        }
    }
}
