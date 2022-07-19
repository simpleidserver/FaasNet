using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class GetDimensionFingerTableRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;

        public GetDimensionFingerTableRequestHandler(IDHTPeerInfoStore peerInfoStore)
        {
            _peerInfoStore = peerInfoStore;
        }

        public ChordCommandTypes Command => ChordCommandTypes.GET_DIMENSION_FINGER_TABLE_REQUEST;

        public Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var peerInfo = _peerInfoStore.Get();
            ChordPackage result = new GetDimensionFingerTableResult(peerInfo.DimensionFingerTable);
            return Task.FromResult(result);
        }
    }
}
