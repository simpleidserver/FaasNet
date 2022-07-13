using FaasNet.CRDT.Client;
using FaasNet.CRDT.Core.SerializedEntities;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.CRDT.Core
{
    public class SyncCRDTEntitiesTimer
    {
        private readonly PeerOptions _peerOptions;
        private readonly CRDTProtocolOptions _options;
        private readonly ISerializedEntityStore _entityStore;
        private readonly IClusterStore _clusterStore;
        private System.Timers.Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;

        public SyncCRDTEntitiesTimer(IOptions<PeerOptions> peerOptions, IOptions<CRDTProtocolOptions> options, ISerializedEntityStore entityStore, IClusterStore clusterStore)
        {
            _peerOptions = peerOptions.Value;
            _options = options.Value;
            _entityStore = entityStore;
            _clusterStore = clusterStore;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _timer = new System.Timers.Timer(_options.SyncCRDTEntitiesTimerMS);
            _timer.Elapsed += async (o, e) => await SyncCRDTEntities(o, e);
            _timer.AutoReset = false;
        }

        public void Stop()
        {
            _timer?.Stop();
            _cancellationTokenSource.Cancel();
        }

        private async Task SyncCRDTEntities(object o, ElapsedEventArgs e)
        {
            var entities = await _entityStore.GetAll(_cancellationTokenSource.Token);
            var clusters = await _clusterStore.GetAllNodes(_cancellationTokenSource.Token);
            clusters = clusters.Where(c => c.Url != _peerOptions.Url || c.Port != _peerOptions.Port).OrderBy(x => Guid.NewGuid());
            var min = Math.Min(_options.MaxBroadcastedPeers, clusters.Count());
            for(var i = 0; i < min; i++)
            {
                var cluster = clusters.ElementAt(i);
                using (var crdtClient = new UDPCRDTClient(cluster.Url, cluster.Port))
                {
                    foreach (var entity in entities) await crdtClient.Sync(_peerOptions.PeerId, entity.Id, entity.ClockVector, _cancellationTokenSource.Token);
                }
            }
        }
    }
}
