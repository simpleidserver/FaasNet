﻿using FaasNet.DHT.Kademlia.Client;
using FaasNet.DHT.Kademlia.Client.Messages;
using FaasNet.DHT.Kademlia.Core.Stores;
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
        private readonly DHTOptions _options;

        public StoreRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore, IOptions<DHTOptions> options)
        {
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
            _options = options.Value;
        }

        public Commands Command => Commands.STORE_REQUEST;

        public async Task<BasePackage> Handle(BasePackage request, CancellationToken cancellationToken)
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
            using (var client = new KademliaClient(targetPeer.Url, targetPeer.Port))
            {
                return await client.StoreValue(storeRequest.Key, storeRequest.Value, cancellationToken);
            }
        }
    }
}
