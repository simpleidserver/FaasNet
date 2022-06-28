using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class GetKeyRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerDataStore _peerDataStore;

        public GetKeyRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore)
        {
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
        }

        public Commands Command => Commands.GET_KEY_REQUEST;

        public Task<DHTPackage> Handle(DHTPackage request, CancellationToken token)
        {
            var getKeyRequest = request as GetKeyRequest;
            var peerInfo = _peerInfoStore.Get();
            if(IntervalHelper.CheckIntervalEquivalence(peerInfo.PredecessorPeer.Id, getKeyRequest.Id, peerInfo.Peer.Id, peerInfo.DimensionFingerTable))
            {
                var peerData = _peerDataStore.Get(getKeyRequest.Id);
                var result = PackageResponseBuilder.GetKey(getKeyRequest.Id, peerData);
                return Task.FromResult(result);
            }

            using (var chordClient = new ChordClient(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
            {
                var successor = chordClient.FindSuccessor(getKeyRequest.Id);
                using(var successorChordClient = new ChordClient(successor.Url, successor.Port))
                {
                    var str = successorChordClient.GetKey(getKeyRequest.Id);
                    var result = PackageResponseBuilder.GetKey(getKeyRequest.Id, str);
                    return Task.FromResult(result);
                }
            }
        }
    }
}
