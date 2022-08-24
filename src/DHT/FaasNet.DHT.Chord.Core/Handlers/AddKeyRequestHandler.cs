using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class AddKeyRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerDataStore _peerDataStore;
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly ChordOptions _options;

        public AddKeyRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore, IPeerClientFactory peerClientFactory, IOptions<ChordOptions> options)
        {
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
            _peerClientFactory = peerClientFactory;
            _options = options.Value;
        }

        public ChordCommandTypes Command => ChordCommandTypes.ADD_KEY_REQUEST;

        public async Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var addKeyRequest = request as AddKeyRequest;
            var peerInfo = _peerInfoStore.Get();
            if (peerInfo.PredecessorPeer == null || peerInfo.SuccessorPeer == null) return PackageResponseBuilder.AddKey();
            if (IntervalHelper.CheckIntervalEquivalence(peerInfo.PredecessorPeer.Id, addKeyRequest.Id, peerInfo.Peer.Id, peerInfo.DimensionFingerTable) || addKeyRequest.Force)
            {
                _peerDataStore.Add(addKeyRequest.Id, addKeyRequest.Value);
                return PackageResponseBuilder.AddKey();
            }

            FindSuccessorResult successor;
            using (var chordClient = _peerClientFactory.Build<ChordClient>(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
                successor = await chordClient.FindSuccessor(addKeyRequest.Id, _options.RequestExpirationTimeMS, token);

            using (var successorChordClient = _peerClientFactory.Build<ChordClient>(successor.Url, successor.Port))
                await successorChordClient.AddKey(addKeyRequest.Id, addKeyRequest.Value, timeoutMS: _options.RequestExpirationTimeMS, cancellationToken: token);

            var result = PackageResponseBuilder.AddKey();
            return result;
        }
    }
}
