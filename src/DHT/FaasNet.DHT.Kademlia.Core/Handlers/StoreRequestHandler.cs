using FaasNet.DHT.Kademlia.Client;
using FaasNet.DHT.Kademlia.Client.Messages;
using FaasNet.DHT.Kademlia.Core.Stores;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core.Handlers
{
    public class StoreRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerDataStore _peerDataStore;
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly KademliaOptions _options;

        public StoreRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore, IPeerClientFactory peerClientFactory, IOptions<KademliaOptions> options)
        {
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
            _peerClientFactory = peerClientFactory;
            _options = options.Value;
        }

        public KademliaCommandTypes Command => KademliaCommandTypes.STORE_REQUEST;

        public async Task<KademliaPackage> Handle(KademliaPackage request, CancellationToken cancellationToken)
        {
            var storeRequest = request as StoreRequest;
            var peerInfo = _peerInfoStore.Get();
            var result = peerInfo.FindClosestPeers(storeRequest.Key, _options.K);
            if (storeRequest.Force) result = result.Where(p => p.PeerId != storeRequest.ExcludedPeer);
            if (!result.Any() || result.First().PeerId == peerInfo.Id)
            {
                _peerDataStore.Add(storeRequest.Key, storeRequest.Value);
                return PackageResponseBuilder.StoreValue(storeRequest.Nonce);
            }

            var targetPeer = result.First();
            using (var client = _peerClientFactory.Build<KademliaClient>(targetPeer.Url, targetPeer.Port))
                return await client.StoreValue(storeRequest.Key, storeRequest.Value, cancellationToken, _options.RequestTimeoutMS);
        }
    }
}
