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
    public class FindValueRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerDataStore _peerDataStore;
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly KademliaOptions _options;

        public FindValueRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore, IPeerClientFactory peerClientFactory, IOptions<KademliaOptions> options)
        {
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
            _peerClientFactory = peerClientFactory;
            _options = options.Value;
        }

        public KademliaCommandTypes Command => KademliaCommandTypes.FIND_VALUE_REQUEST;

        public async Task<KademliaPackage> Handle(KademliaPackage request, CancellationToken cancellationToken)
        {
            var findValueRequest = request as FindValueRequest;
            if (_peerDataStore.TryGet(findValueRequest.Key, out string value)) return PackageResponseBuilder.FindValue(findValueRequest.Key, value, findValueRequest.Nonce);
            var peerInfo =_peerInfoStore.Get();
            var result = peerInfo.FindClosestPeers(findValueRequest.Key, 1);
            if (!result.Any() || result.First().PeerId == peerInfo.Id) return PackageResponseBuilder.FindValue(findValueRequest.Key, string.Empty, findValueRequest.Nonce);
            var targetPeer = result.First();
            using (var client = _peerClientFactory.Build<KademliaClient>(targetPeer.Url, targetPeer.Port))
                return await client.FindValue(findValueRequest.Key, cancellationToken, _options.RequestTimeoutMS);
        }
    }
}
