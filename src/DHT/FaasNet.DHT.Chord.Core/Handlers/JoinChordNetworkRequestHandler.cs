using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class JoinChordNetworkRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly ChordOptions _options;

        public JoinChordNetworkRequestHandler(IDHTPeerInfoStore peerInfoStore, IPeerClientFactory peerClientFactory, IOptions<ChordOptions> options)
        {
            _peerInfoStore = peerInfoStore;
            _peerClientFactory = peerClientFactory;
            _options = options.Value;
        }

        public ChordCommandTypes Command => ChordCommandTypes.JOIN_CHORD_NETWORK_REQUEST;

        public async Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var joinChordNetwork = request as JoinChordNetworkRequest;
            int dimFingerTable = 0;
            using (var chordClient = _peerClientFactory.Build<ChordClient>(joinChordNetwork.Url, joinChordNetwork.Port))
                dimFingerTable = await chordClient.GetDimensionFingerTable(_options.RequestExpirationTimeMS, token);

            var peerInfo = _peerInfoStore.Get();
            peerInfo.ComputeId(dimFingerTable);
            FindSuccessorResult successorNode;
            using (var secondChordClient = _peerClientFactory.Build<ChordClient>(joinChordNetwork.Url, joinChordNetwork.Port))
                successorNode = await secondChordClient.FindSuccessor(peerInfo.Peer.Id, _options.RequestExpirationTimeMS, token);

            peerInfo.SuccessorPeer = new PeerInfo { Id = successorNode.Id, Port = successorNode.Port, Url = successorNode.Url };
            _peerInfoStore.Update(peerInfo);
            var result = PackageResponseBuilder.Join();
            return result;
        }
    }
}
