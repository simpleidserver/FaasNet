using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class JoinChordNetworkRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;

        public JoinChordNetworkRequestHandler(IDHTPeerInfoStore peerInfoStore)
        {
            _peerInfoStore = peerInfoStore;
        }

        public Commands Command => Commands.JOIN_CHORD_NETWORK_REQUEST;

        public async Task<DHTPackage> Handle(DHTPackage request, CancellationToken token)
        {
            var joinChordNetwork = request as JoinChordNetworkRequest;
            var peerInfo = _peerInfoStore.Get();
            using (var chordClient = new ChordClient(joinChordNetwork.Url, joinChordNetwork.Port))
            {
                var dimFingerTable = chordClient.GetDimensionFingerTable();
                var successorNode = chordClient.FindSuccessor(peerInfo.Id);
                _peerInfoStore.Update(peerInfo);
            }

            throw new System.NotImplementedException();
        }
    }
}
