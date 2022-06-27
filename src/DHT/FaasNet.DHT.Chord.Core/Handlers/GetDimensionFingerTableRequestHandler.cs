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

        public Commands Command => Commands.GET_DIMENSION_FINGER_TABLE_REQUEST;

        public Task<DHTPackage> Handle(DHTPackage request, CancellationToken token)
        {
            var peerInfo = _peerInfoStore.Get();
            DHTPackage result = new GetDimensionFingerTableResult(peerInfo.DimensionFingerTable);
            return Task.FromResult(result);
        }
    }
}
