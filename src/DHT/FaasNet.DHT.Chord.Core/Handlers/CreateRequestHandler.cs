using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class CreateRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore;

        public CreateRequestHandler(IDHTPeerInfoStore peerInfoStore)
        {
            _peerInfoStore = peerInfoStore;
        }

        public ChordCommandTypes Command => ChordCommandTypes.CREATE_REQUEST;

        public Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var createRequest = request as CreateRequest;
            var peerInfo = _peerInfoStore.Get();
            peerInfo.ComputeId(createRequest.DimFingerTable);
            _peerInfoStore.Update(peerInfo);
            var result = PackageResponseBuilder.Create();
            return Task.FromResult(result);
        }
    }
}
