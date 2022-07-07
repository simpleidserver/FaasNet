﻿using FaasNet.DHT.Kademlia.Client;
using FaasNet.DHT.Kademlia.Client.Messages;
using FaasNet.DHT.Kademlia.Core.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core.Handlers
{
    public class FindValueRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerDataStore _peerDataStore;

        public FindValueRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore)
        {
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
        }

        public Commands Command => Commands.FIND_VALUE_REQUEST;

        public async Task<BasePackage> Handle(BasePackage request, CancellationToken cancellationToken)
        {
            var findValueRequest = request as FindValueRequest;
            if (_peerDataStore.TryGet(findValueRequest.Key, out string value)) return PackageResponseBuilder.FindValue(findValueRequest.Key, value, findValueRequest.Nonce);
            var peerInfo =_peerInfoStore.Get();
            var result = peerInfo.FindClosestPeers(findValueRequest.Key, 1);
            if (!result.Any() || result.First().PeerId == peerInfo.Id) return PackageResponseBuilder.FindValue(findValueRequest.Key, string.Empty, findValueRequest.Nonce);
            var targetPeer = result.First();
            using (var client = new KademliaClient(targetPeer.Url, targetPeer.Port))
            {
                return await client.FindValue(findValueRequest.Key, cancellationToken);
            }
        }
    }
}