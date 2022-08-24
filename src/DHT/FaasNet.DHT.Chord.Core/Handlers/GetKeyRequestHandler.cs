using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class GetKeyRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerDataStore _peerDataStore;
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly ChordOptions _options;

        public GetKeyRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore, IPeerClientFactory peerClientFactory, IOptions<ChordOptions> options)
        {
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
            _peerClientFactory = peerClientFactory;
            _options = options.Value;
        }

        public ChordCommandTypes Command => ChordCommandTypes.GET_KEY_REQUEST;

        public async Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var getKeyRequest = request as GetKeyRequest;
            var peerInfo = _peerInfoStore.Get();
            if (IntervalHelper.CheckIntervalEquivalence(peerInfo.PredecessorPeer.Id, getKeyRequest.Id, peerInfo.Peer.Id, peerInfo.DimensionFingerTable))
            {
                var peerData = _peerDataStore.Get(getKeyRequest.Id);
                return PackageResponseBuilder.GetKey(getKeyRequest.Id, peerData);
            }

            FindSuccessorResult successor;
            using (var chordClient = _peerClientFactory.Build<ChordClient>(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
                successor = await chordClient.FindSuccessor(getKeyRequest.Id, _options.RequestExpirationTimeMS, token);

            string str;
            using (var successorChordClient = _peerClientFactory.Build<ChordClient>(successor.Url, successor.Port))
                str = await successorChordClient.GetKey(getKeyRequest.Id, _options.RequestExpirationTimeMS, token);

            var result = PackageResponseBuilder.GetKey(getKeyRequest.Id, str);
            return result;
        }
    }
}
