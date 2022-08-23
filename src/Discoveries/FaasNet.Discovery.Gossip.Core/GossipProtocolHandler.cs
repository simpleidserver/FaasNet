using FaasNet.Discovery.Gossip.Client.Messages;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Discovery.Gossip.Core
{
    public class GossipProtocolHandler : IProtocolHandler
    {
        private readonly IClusterStore _clusterStore;

        public GossipProtocolHandler(IClusterStore clusterStore)
        {
            _clusterStore = clusterStore;
        }

        public string MagicCode => GossipPackage.MAGIC_CODE;

        public async Task<BasePeerPackage> Handle(byte[] payload, CancellationToken cancellationToken)
        {
            var context = new ReadBufferContext(payload);
            GossipPackage gossipPackage = GossipPackage.Deserialize(context);
            if (gossipPackage.Type == GossipPackageTypes.SYNC) return await Handle(gossipPackage as GossipSyncPackage, cancellationToken);
            if (gossipPackage.Type == GossipPackageTypes.GET) return await Handle(gossipPackage as GossipGetPackage, cancellationToken);
            return GossipPackageResultBuilder.Ok();
        }

        private async Task<BasePeerPackage> Handle(GossipSyncPackage gossipSyncPackage, CancellationToken cancellationToken)
        {
            foreach (var peerInfo in gossipSyncPackage.PeerInfos) await _clusterStore.SelfRegister(new ClusterPeer(peerInfo.Url, peerInfo.Port), cancellationToken);
            return GossipPackageResultBuilder.Ok();
        }

        private async Task<BasePeerPackage> Handle(GossipGetPackage gossipGetPackage, CancellationToken cancellationToken)
        {
            var allNodes = await _clusterStore.GetAllNodes(gossipGetPackage.PartitionKey, cancellationToken);
            return GossipPackageResultBuilder.Get(allNodes.Select(n => new PeerInfo { Port = n.Port, Url = n.Url }).ToList());
        }
    }
}
