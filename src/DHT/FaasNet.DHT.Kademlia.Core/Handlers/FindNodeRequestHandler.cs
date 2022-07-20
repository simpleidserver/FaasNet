using FaasNet.DHT.Kademlia.Client.Messages;
using FaasNet.DHT.Kademlia.Core.Stores;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core.Handlers
{
    public class FindNodeRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly KademliaOptions _options;

        public FindNodeRequestHandler(IDHTPeerInfoStore peerInfoStore, IOptions<KademliaOptions> options)
        {
            _peerInfoStore = peerInfoStore;
            _options = options.Value;
        }

        public KademliaCommandTypes Command => KademliaCommandTypes.FIND_NODE_REQUEST;

        public Task<KademliaPackage> Handle(KademliaPackage request, CancellationToken cancellationToken)
        {
            var findNodeRequest = request as FindNodeRequest;
            var peerInfo = _peerInfoStore.Get();
            peerInfo.TryAddPeer(findNodeRequest.Url, findNodeRequest.Port, findNodeRequest.Id);
            _peerInfoStore.Update(peerInfo);
            var peers = peerInfo.FindClosestPeers(findNodeRequest.TargetId, _options.K);
            return Task.FromResult(PackageResponseBuilder.FindNode(peers.Select(p => new PeerResult
            {
                Id = p.PeerId,
                Port = p.Port,
                Url = p.Url
            }).ToList(), request.Nonce));
        }
    }
}
