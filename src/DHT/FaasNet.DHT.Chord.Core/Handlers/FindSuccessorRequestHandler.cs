using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class FindSuccessorRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly ChordOptions _options;

        public FindSuccessorRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerClientFactory peerClientFactory, IOptions<ChordOptions> options)
        {
            _peerInfoStore = peerInfoStore;
            _peerClientFactory = peerClientFactory;
            _options = options.Value;
        }

        public ChordCommandTypes Command => ChordCommandTypes.FIND_SUCCESSOR_REQUEST;

        public async Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var findSuccessorRequest = request as FindSuccessorRequest;
            var dhtPeerInfo = _peerInfoStore.Get();
            var successor = await FindSuccessor(findSuccessorRequest.NodeId, dhtPeerInfo, token);
            var result = PackageResponseBuilder.FindSuccessor(successor.Url, successor.Port, successor.Id);
            return result;
        }

        private async Task<PeerInfo> FindSuccessor(long target, DHTPeerInfo peerInfo, CancellationToken token)
        {
            if(peerInfo.SuccessorPeer != null && IntervalHelper.CheckIntervalEquivalence(peerInfo.Peer.Id, target, peerInfo.SuccessorPeer.Id, peerInfo.DimensionFingerTable))
                return peerInfo.SuccessorPeer;

            var precedingPeer = ClosestPrecedingPeer(target, peerInfo);
            if (precedingPeer.Id == peerInfo.Peer.Id) return precedingPeer;
            using (var chordClient = _peerClientFactory.Build<ChordClient>(precedingPeer.Url, precedingPeer.Port))
            {
                var result = await chordClient.FindSuccessor(target, _options.RequestExpirationTimeMS, token);
                return new PeerInfo { Id = result.Id, Port = result.Port, Url = result.Url };
            }
        }

        private PeerInfo ClosestPrecedingPeer(long target, DHTPeerInfo peerInfo)
        {
            for(var i = peerInfo.Fingers.Count() - 1; i >= 0; i--)
            {
                var finger = peerInfo.Fingers.ElementAt(i);
                if(IntervalHelper.CheckIntervalClosest(finger.Peer.Id, target, peerInfo.Peer.Id, peerInfo.DimensionFingerTable))
                {
                    return finger.Peer;
                }
            }

            return peerInfo.Peer;
        }
    }
}
