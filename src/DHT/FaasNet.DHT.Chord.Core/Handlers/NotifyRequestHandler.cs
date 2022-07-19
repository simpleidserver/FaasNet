using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core.Handlers
{
    public class NotifyRequestHandler : IRequestHandler
    {
        private readonly IDHTPeerInfoStore _peerInfoStore; 

        public NotifyRequestHandler(IDHTPeerInfoStore peerInfoStore)
        {
            _peerInfoStore = peerInfoStore;
        }

        public ChordCommandTypes Command => ChordCommandTypes.NOTIFY_REQUEST;

        public Task<ChordPackage> Handle(ChordPackage request, CancellationToken token)
        {
            var notifyRequest = request as NotifyRequest;
            var peerInfo = _peerInfoStore.Get();
            if (peerInfo.PredecessorPeer == null || IntervalHelper.CheckInterval(peerInfo, notifyRequest.Id))
            {
                peerInfo.PredecessorPeer = new PeerInfo { Id = notifyRequest.Id, Port = notifyRequest.Port, Url = notifyRequest.Url };
            }

            if (peerInfo.SuccessorPeer == null)
            {
                peerInfo.SuccessorPeer = new PeerInfo { Id = notifyRequest.Id, Port = notifyRequest.Port, Url = notifyRequest.Url };
            }

            var result = PackageResponseBuilder.Notify();
            return Task.FromResult(result);
        }
    }
}
