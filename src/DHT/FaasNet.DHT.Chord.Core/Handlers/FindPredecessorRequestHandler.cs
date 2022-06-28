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

        public Commands Command => Commands.FIND_PREDECESSOR_REQUEST;

        public Task<DHTPackage> Handle(DHTPackage request, CancellationToken token)
        {
            var predecessor = _peerInfoStore.Get().PredecessorPeer;
            var result = PackageResponseBuilder.NotFoundPredecessor();
            if (predecessor != null) result = PackageResponseBuilder.FindPredecessor(predecessor.Id, predecessor.Url, predecessor.Port);
            return Task.FromResult(result);
        }
    }
}
