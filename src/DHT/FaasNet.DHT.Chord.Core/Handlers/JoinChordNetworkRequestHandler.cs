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
            using (var chordClient = new ChordClient(joinChordNetwork.Url, joinChordNetwork.Port))
            {
                var dimFingerTable = chordClient.GetDimensionFingerTable();
                var peerInfo = _peerInfoStore.Get();
                peerInfo.ComputeId(dimFingerTable);
                var successorNode = chordClient.FindSuccessor(peerInfo.Peer.Id);
                peerInfo.SuccessorPeer = new PeerInfo { Id = successorNode.Id, Port = successorNode.Port, Url = successorNode.Url };
                _peerInfoStore.Update(peerInfo);
            }

            var result = PackageResponseBuilder.Join();
            return result;
        }
    }
}
