using FaasNet.DHT.Kademlia.Client;
using FaasNet.DHT.Kademlia.Client.Messages;
using FaasNet.DHT.Kademlia.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core.Handlers
{
    public class JoinRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;

        public JoinRequestHandler(IDHTPeerInfoStore peerInfoStore)
        {
            _peerInfoStore = peerInfoStore;
        }

        public Commands Command => Commands.JOIN_REQUEST;

        public async Task<BasePackage> Handle(BasePackage request, CancellationToken cancellationToken)
        {
            var joinRequest = request as JoinRequest;
            var peerInfo = _peerInfoStore.Get();
            using (var client = new KademliaClient(joinRequest.Url, joinRequest.Port))
            {
                var findResult = await client.FindNode(peerInfo.Id, peerInfo.Url, peerInfo.Port, peerInfo.Id, cancellationToken);
                foreach (var peer in findResult.Peers) peerInfo.TryAddPeer(peer.Url, peer.Port, peer.Id);
                _peerInfoStore.Update(peerInfo);
            }

            return PackageResponseBuilder.Join(joinRequest.Nonce);
        }
    }
}
