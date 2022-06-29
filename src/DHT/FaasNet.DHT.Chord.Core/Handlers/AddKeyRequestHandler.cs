using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class AddKeyRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerDataStore _peerDataStore;

        public AddKeyRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore)
        {
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
        }

        public Commands Command => Commands.ADD_KEY_REQUEST;

        public Task<DHTPackage> Handle(DHTPackage request, CancellationToken token)
        {
            var addKeyRequest = request as AddKeyRequest;
            var peerInfo = _peerInfoStore.Get();
            if (IntervalHelper.CheckIntervalEquivalence(peerInfo.PredecessorPeer.Id, addKeyRequest.Id, peerInfo.Peer.Id, peerInfo.DimensionFingerTable) || addKeyRequest.Force)
            {
                _peerDataStore.Add(addKeyRequest.Id, addKeyRequest.Value);
                var result = PackageResponseBuilder.AddKey();
                return Task.FromResult(result);
            }

            using (var chordClient = new ChordClient(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
            {
                var successor = chordClient.FindSuccessor(addKeyRequest.Id);
                using (var successorChordClient = new ChordClient(successor.Url, successor.Port))
                {
                    successorChordClient.AddKey(addKeyRequest.Id, addKeyRequest.Value);
                    var result = PackageResponseBuilder.AddKey();
                    return Task.FromResult(result);
                }
            }
        }
    }
}
