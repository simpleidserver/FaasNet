﻿using FaasNet.DHT.Kademlia.Client;
using FaasNet.DHT.Kademlia.Core.Stores;
using FaasNet.Peer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core
{
    public class KademliaTimer : ITimer
    {
        private readonly ILogger<KademliaTimer> _logger;
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerDataStore _peerDataStore;
        private readonly PeerOptions _peerOptions;
        private readonly KademliaOptions _options;
        private CancellationTokenSource _cancellationTokenSource;
        private System.Timers.Timer _fixKBucketLstTimer;
        private System.Timers.Timer _healthCheckTimer;

        public KademliaTimer(ILogger<KademliaTimer> logger, IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore, IOptions<PeerOptions> peerOptions, IOptions<KademliaOptions> options)
        {
            _logger = logger;
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
            _peerOptions = peerOptions.Value;
            _options = options.Value;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            var id = long.Parse(_peerOptions.PeerId);
            var peerInfoStore = KademliaPeerInfo.Create(id, _peerOptions.Url, _peerOptions.Port, _options.S);
            peerInfoStore.TryAddPeer(_peerOptions.Url, _peerOptions.Port, id);
            _peerInfoStore.Update(peerInfoStore);
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _fixKBucketLstTimer = new System.Timers.Timer(_options.FixKBucketLstTimerMS);
            _fixKBucketLstTimer.Elapsed += async (o, e) => await FixKBucketList();
            _fixKBucketLstTimer.AutoReset = false;
            _fixKBucketLstTimer.Start();
            _healthCheckTimer = new System.Timers.Timer(_options.HealthCheckTimerMS);
            _healthCheckTimer.Elapsed += async (o, e) => await HealthCheck();
            _healthCheckTimer.AutoReset = false;
            _healthCheckTimer.Start();
            return Task.CompletedTask;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _fixKBucketLstTimer.Stop();
            _healthCheckTimer.Stop();
        }

        private async Task FixKBucketList()
        {
            try
            {
                if (!_options.IsSeedPeer)
                {
                    var peerInfo = _peerInfoStore.Get();
                    using (var client = new UDPKademliaClient(_options.SeedUrl, _options.SeedPort))
                    {
                        var findResult = await client.FindNode(peerInfo.Id, peerInfo.Url, peerInfo.Port, peerInfo.Id, _cancellationTokenSource.Token);
                        foreach (var peer in findResult.Peers) peerInfo.TryAddPeer(peer.Url, peer.Port, peer.Id);
                    }

                    _peerInfoStore.Update(peerInfo);
                }

                await TryTransferDataToClosestPeers();
                _fixKBucketLstTimer.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _fixKBucketLstTimer.Start();
            }
        }

        private async Task HealthCheck()
        {
            var peerInfo = _peerInfoStore.Get();
            var peers = peerInfo.KBucketLst.SelectMany(b => b.Peers);
            foreach (var peer in peers)
            {
                try
                {
                    using (var client = new UDPKademliaClient(peer.Url, peer.Port))
                    {
                        await client.Ping();
                        peer.Enable();
                    }
                }
                catch
                {
                    peer.Disable();
                }

                _peerInfoStore.Update(peerInfo);
            }

            _healthCheckTimer.Start();
        }

        private async Task TryTransferDataToClosestPeers()
        {
            var peerInfo = _peerInfoStore.Get();
            var allData = _peerDataStore.GetAll();
            foreach (var data in allData)
            {
                var result = peerInfo.FindClosestPeers(data.Id, 1);
                if (!result.Any() || result.First().PeerId == peerInfo.Id) continue;
                var targetPeer = result.First();
                using (var client = new UDPKademliaClient(targetPeer.Url, targetPeer.Port))
                    await client.StoreValue(data.Id, data.Value, _cancellationTokenSource.Token);

                _peerDataStore.TryRemove(data);
            }
        }

        private async Task TransferAllData()
        {
            var peerInfo = _peerInfoStore.Get();
            var allData = _peerDataStore.GetAll();
            foreach (var data in allData)
            {
                var result = peerInfo.FindClosestPeers(data.Id, _options.K).Where(p => p.PeerId != peerInfo.Id);
                if (!result.Any()) continue;
                var targetPeer = result.First();
                using (var client = new UDPKademliaClient(targetPeer.Url, targetPeer.Port))
                    await client.ForceStoreValue(data.Id, data.Value, peerInfo.Id, _cancellationTokenSource.Token);
            }
        }
    }
}