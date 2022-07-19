using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class FindPredecessorRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;

        public FindPredecessorRequestHandler(IDHTPeerInfoStore peerInfoStore)
        {
            _peerInfoStore = peerInfoStore;
        }

        public ChordCommandTypes Command => ChordCommandTypes.FIND_PREDECESSOR_REQUEST;

        public Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var peerInfo = _peerInfoStore.Get();
            var predecessor = peerInfo.PredecessorPeer;
            var result = PackageResponseBuilder.NotFoundPredecessor();
            if (predecessor != null) result = PackageResponseBuilder.FindPredecessor(predecessor.Id, predecessor.Url, predecessor.Port);
            return Task.FromResult(result);
        }
    }
}
