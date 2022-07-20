using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using System.Diagnostics;
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

        public ChordCommandTypes Command => ChordCommandTypes.JOIN_CHORD_NETWORK_REQUEST;

        public async Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var joinChordNetwork = request as JoinChordNetworkRequest;
            int dimFingerTable = 0;
            using (var chordClient = new TCPChordClient(joinChordNetwork.Url, joinChordNetwork.Port))
            {
                dimFingerTable = chordClient.GetDimensionFingerTable();
            }

            var peerInfo = _peerInfoStore.Get();
            peerInfo.ComputeId(dimFingerTable);
            FindSuccessorResult successorNode;
            using (var secondChordClient = new TCPChordClient(joinChordNetwork.Url, joinChordNetwork.Port))
            {
                successorNode = secondChordClient.FindSuccessor(peerInfo.Peer.Id);
            }

            peerInfo.SuccessorPeer = new PeerInfo { Id = successorNode.Id, Port = successorNode.Port, Url = successorNode.Url };
            _peerInfoStore.Update(peerInfo);
            var result = PackageResponseBuilder.Join();
            return result;
        }
    }
}
