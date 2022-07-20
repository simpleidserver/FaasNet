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

        public ChordCommandTypes Command => ChordCommandTypes.ADD_KEY_REQUEST;

        public Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var addKeyRequest = request as AddKeyRequest;
            var peerInfo = _peerInfoStore.Get();
            if (peerInfo.PredecessorPeer == null || peerInfo.SuccessorPeer == null) return Task.FromResult(PackageResponseBuilder.AddKey());
            if (IntervalHelper.CheckIntervalEquivalence(peerInfo.PredecessorPeer.Id, addKeyRequest.Id, peerInfo.Peer.Id, peerInfo.DimensionFingerTable) || addKeyRequest.Force)
            {
                _peerDataStore.Add(addKeyRequest.Id, addKeyRequest.Value);
                return Task.FromResult(PackageResponseBuilder.AddKey());
            }

            FindSuccessorResult successor;
            using (var chordClient = new TCPChordClient(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
            {
                successor = chordClient.FindSuccessor(addKeyRequest.Id);
            }

            using (var successorChordClient = new TCPChordClient(successor.Url, successor.Port))
            {
                successorChordClient.AddKey(addKeyRequest.Id, addKeyRequest.Value);
            }

            var result = PackageResponseBuilder.AddKey();
            return Task.FromResult(result);
        }
    }
}
