using FaasNet.Discovery.Gossip.Client;
using FaasNet.Discovery.Gossip.Client.Messages;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.Discovery.Gossip.Core
{
    public class GossipTimer : ITimer
    {
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly GossipOptions _gossipOptions;
        private readonly PeerOptions _peerOptions;
        private readonly IClusterStore _clusterStore;
        private readonly ILogger<GossipTimer> _logger;
        private CancellationTokenSource _cancellationTokenSource;
        private System.Timers.Timer _timer;

        public GossipTimer(IPeerClientFactory peerClientFactory, IOptions<GossipOptions> gossipOptions, IOptions<PeerOptions> peerOptions, IClusterStore clusterStore, ILogger<GossipTimer> logger)
        {
            _peerClientFactory = peerClientFactory;
            _gossipOptions = gossipOptions.Value;
            _peerOptions = peerOptions.Value;
            _clusterStore = clusterStore;
            _logger = logger;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _timer = new System.Timers.Timer(_gossipOptions.BroadcastRumorToRandomNeighbourTimerMS);
            _timer.Elapsed += async(o, e) => await BroadcastRumor(o, e);
            _timer.AutoReset = false;
            StartBroadcastRumor();
            return Task.CompletedTask;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _timer.Stop();
        }

        private void StartBroadcastRumor()
        {
            _timer.Start();
        }

        private async Task BroadcastRumor(object o, ElapsedEventArgs e)
        {
            var peers = (await _clusterStore.GetAllNodes(null, _cancellationTokenSource.Token)).Where(p => p.Url != _peerOptions.Url || p.Port != _peerOptions.Port);
            var filteredPeers = peers.OrderBy(c => Guid.NewGuid()).Take(_gossipOptions.MaxNbPeersToBroadcastMessage);
            var peerInfos = peers.Select(p => new PeerInfo { Port = p.Port, Url = p.Url }).ToList();
            peerInfos.Add(new PeerInfo { Url = _peerOptions.Url, Port = _peerOptions.Port });
            var tasks = peers.Select(p => BroadcastRumor(p, peerInfos));
            await Task.WhenAll(tasks);
            StartBroadcastRumor();
        }

        private async Task BroadcastRumor(ClusterPeer clusterPeer, ICollection<PeerInfo> peers)
        {
            try
            {
                using (var gossipClient = _peerClientFactory.Build<GossipClient>(clusterPeer.Url, clusterPeer.Port))
                {
                    await gossipClient.Sync(peers, _cancellationTokenSource.Token);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
