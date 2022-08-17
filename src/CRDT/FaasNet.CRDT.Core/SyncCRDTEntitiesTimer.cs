using FaasNet.CRDT.Client;
using FaasNet.CRDT.Core.Entities;
using FaasNet.CRDT.Core.SerializedEntities;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.CRDT.Core
{
    public class SyncCRDTEntitiesTimer : ITimer
    {
        private readonly PeerOptions _peerOptions;
        private readonly CRDTProtocolOptions _options;
        private readonly ISerializedEntityStore _entityStore;
        private readonly IClusterStore _clusterStore;
        private readonly ICRDTEntityFactory _entityFactory;
        private readonly ILogger<SyncCRDTEntitiesTimer> _logger;
        private System.Timers.Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;

        public SyncCRDTEntitiesTimer(IOptions<PeerOptions> peerOptions, IOptions<CRDTProtocolOptions> options, ISerializedEntityStore entityStore, IClusterStore clusterStore, ICRDTEntityFactory entityFactory, ILogger<SyncCRDTEntitiesTimer> logger)
        {
            _peerOptions = peerOptions.Value;
            _options = options.Value;
            _entityStore = entityStore;
            _clusterStore = clusterStore;
            _entityFactory = entityFactory;
            _logger = logger;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _timer = new System.Timers.Timer(_options.SyncCRDTEntitiesTimerMS);
            _timer.Elapsed += async (o, e) => await SyncCRDTEntities(o, e);
            _timer.AutoReset = false;
            StartSyncCRDTEntities();
            return Task.CompletedTask;
        }

        public void Stop()
        {
            _timer?.Stop();
            _cancellationTokenSource.Cancel();
        }

        private void StartSyncCRDTEntities()
        {
            _timer?.Start();
        }

        private async Task SyncCRDTEntities(object o, ElapsedEventArgs e)
        {
            var entities = await _entityStore.GetAll(_cancellationTokenSource.Token);
            var clusters = await _clusterStore.GetAllNodes(_cancellationTokenSource.Token);
            clusters = clusters.Where(c => c.Url != _peerOptions.Url || c.Port != _peerOptions.Port).OrderBy(x => Guid.NewGuid());
            var min = Math.Min(_options.MaxBroadcastedPeers, clusters.Count());
            var lst = entities.ToDictionary(e => e.Id, e => _entityFactory.Build(e));
            var taskLst = clusters.Select(c => SyncCRDTEntities(c, lst));
            await Task.WhenAll(taskLst);
            foreach(var kvp in lst)
            {
                var serializedEntity = new CRDTEntitySerializer().Serialize(kvp.Key, kvp.Value);
                await _entityStore.Update(serializedEntity, _cancellationTokenSource.Token);
            }

            StartSyncCRDTEntities();
        }

        private async Task SyncCRDTEntities(ClusterPeer clusterPeer, Dictionary<string, CRDTEntity> lst)
        {
            using (var crdtClient = new UDPCRDTClient(clusterPeer.Url, clusterPeer.Port))
            {
                try
                {
                    foreach (var kvp in lst)
                    {
                        var syncResultPackage = await crdtClient.Sync(_peerOptions.Id, kvp.Key, kvp.Value.ClockVector, _cancellationTokenSource.Token);
                        foreach (var diff in syncResultPackage.DiffLst) kvp.Value.ApplyDelta(diff.PeerId, diff.Delta);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }
    }
}
