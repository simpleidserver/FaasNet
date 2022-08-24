using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Core.Stores;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core
{
    public class ChordTimer : ITimer
    {
        private readonly PeerOptions _peerOptions;
        private readonly ChordOptions _options;
        private readonly IPeerDataStore _peerDataStore;
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly ILogger<ChordTimer> _logger;
        private SemaphoreSlim _lockTimer;
        private System.Timers.Timer _stabilizeTimer;
        private System.Timers.Timer _fixFingersTimer;
        private System.Timers.Timer _checkPredecessorAndSuccessorTimer;
        private CancellationTokenSource _cancellationTokenSource;

        public ChordTimer(IOptions<PeerOptions> peerOptions, IOptions<ChordOptions> options, IPeerDataStore peerDataStore, IDHTPeerInfoStore peerInfoStore, IPeerClientFactory peerClientFactory, ILogger<ChordTimer> logger)
        {
            _peerOptions = peerOptions.Value;
            _options = options.Value;
            _peerDataStore = peerDataStore;
            _peerInfoStore = peerInfoStore;
            _peerClientFactory = peerClientFactory;
            _logger = logger;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var peerInfo = new DHTPeerInfo(_peerOptions.Url, _peerOptions.Port);
            _lockTimer = new SemaphoreSlim(1);
            _peerInfoStore.Update(peerInfo);
            _stabilizeTimer = new System.Timers.Timer(_options.StabilizeTimerMS);
            _stabilizeTimer.Elapsed += async (o, e) => await Stabilize();
            _stabilizeTimer.AutoReset = false;
            _stabilizeTimer.Start();
            _fixFingersTimer = new System.Timers.Timer(_options.FixFingersTimerMS);
            _fixFingersTimer.Elapsed += async (o, e) => await FixFingers();
            _fixFingersTimer.AutoReset = false;
            _fixFingersTimer.Start();
            _checkPredecessorAndSuccessorTimer = new System.Timers.Timer(_options.CheckPredecessorAndSuccessorTimerMS);
            _checkPredecessorAndSuccessorTimer.Elapsed += async (o, e) => await CheckPredecessorAndSuccessor();
            _checkPredecessorAndSuccessorTimer.AutoReset = false;
            _checkPredecessorAndSuccessorTimer.Start();
            return Task.CompletedTask;
        }

        public async void Stop()
        {
            _cancellationTokenSource?.Cancel();
            await ForceTransferData();
            _stabilizeTimer.Stop();
            _fixFingersTimer.Stop();
            _checkPredecessorAndSuccessorTimer.Stop();
        }

        private async Task Stabilize()
        {
            _lockTimer.Wait();
            var peerInfo = _peerInfoStore.Get();
            if (peerInfo.SuccessorPeer == null)
            {
                _lockTimer.Release();
                _stabilizeTimer.Start();
                return;
            }

            try
            {
                using (var chordClient = _peerClientFactory.Build<ChordClient>(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
                {
                    var predecessor = await chordClient.FindPredecessor(_options.RequestExpirationTimeMS, _cancellationTokenSource.Token);
                    if (predecessor.HasPredecessor)
                    {
                        if (IntervalHelper.CheckInterval(peerInfo.Peer.Id, predecessor.Id, peerInfo.SuccessorPeer.Id, peerInfo.DimensionFingerTable))
                        {
                            peerInfo.SuccessorPeer = new PeerInfo { Id = predecessor.Id, Port = predecessor.Port, Url = predecessor.Url };
                            _peerInfoStore.Update(peerInfo);
                        }
                    }
                }

                using (var secondClient = _peerClientFactory.Build<ChordClient>(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
                    await secondClient.Notify(peerInfo.Peer.Url, peerInfo.Peer.Port, peerInfo.Peer.Id, _options.RequestExpirationTimeMS, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            _stabilizeTimer.Start();
            _lockTimer.Release();
        }

        private async Task FixFingers()
        {
            _lockTimer.Wait();
            var peerInfo = _peerInfoStore.Get();
            var start = peerInfo.Peer.Id;
            var fingers = new List<FingerTableRecord>();
            if (peerInfo.SuccessorPeer == null)
            {
                for (int i = 1; i <= peerInfo.DimensionFingerTable; i++)
                {
                    var newId = peerInfo.Peer.Id + (long)Math.Pow(2, i - 1);
                    fingers.Add(new FingerTableRecord
                    {
                        Start = start,
                        End = newId,
                        Peer = new PeerInfo { Id = peerInfo.Peer.Id, Port = peerInfo.Peer.Port, Url = peerInfo.Peer.Url }
                    });
                }
            }
            else
            {
                var max = Math.Pow(2, peerInfo.DimensionFingerTable);
                try
                {
                    for (int i = 1; i <= peerInfo.DimensionFingerTable; i++)
                    {
                        using (var chordClient = _peerClientFactory.Build<ChordClient>(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
                        {
                            var newId = peerInfo.Peer.Id + (long)Math.Pow(2, i - 1);
                            if (newId >= max) break;
                            var successor = await chordClient.FindSuccessor(newId, _options.RequestExpirationTimeMS, _cancellationTokenSource.Token);
                            fingers.Add(new FingerTableRecord
                            {
                                Start = start,
                                End = newId,
                                Peer = new PeerInfo { Id = successor.Id, Port = successor.Port, Url = successor.Url }
                            });
                            start = newId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            peerInfo.Fingers = fingers;
            _peerInfoStore.Update(peerInfo);
            await TransferData(peerInfo);
            _fixFingersTimer.Start();
            _lockTimer.Release();
        }

        private async Task CheckPredecessorAndSuccessor()
        {
            _lockTimer.Wait();
            var peerInfo = _peerInfoStore.Get();
            if (peerInfo.PredecessorPeer != null)
            {
                try
                {
                    using (var chordClient = _peerClientFactory.Build<ChordClient>(peerInfo.PredecessorPeer.Url, peerInfo.PredecessorPeer.Port))
                        await chordClient.GetDimensionFingerTable(_options.RequestExpirationTimeMS, _cancellationTokenSource.Token);
                }
                catch
                {
                    peerInfo.PredecessorPeer = null;
                }
            }

            if (peerInfo.SuccessorPeer != null)
            {
                try
                {
                    using (var chordClient = _peerClientFactory.Build<ChordClient>(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
                        await chordClient.GetDimensionFingerTable(_options.RequestExpirationTimeMS, _cancellationTokenSource.Token);
                }
                catch
                {
                    peerInfo.SuccessorPeer = null;
                }
            }

            _peerInfoStore.Update(peerInfo);
            _checkPredecessorAndSuccessorTimer.Start();
            _lockTimer.Release();
        }

        private async Task ForceTransferData()
        {
            var allData = _peerDataStore.GetAll();
            var peerInfo = _peerInfoStore.Get();
            if (peerInfo.SuccessorPeer == null) return;
            using (var chordClient = _peerClientFactory.Build<ChordClient>(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
                foreach (var data in allData) await chordClient.AddKey(data.Id, data.Value, true, _options.RequestExpirationTimeMS, _cancellationTokenSource.Token);
        }

        private async Task TransferData(DHTPeerInfo peerInfo)
        {
            if (peerInfo.SuccessorPeer == null || peerInfo.PredecessorPeer == null) return;
            var allData = _peerDataStore.GetAll();
            try
            {
                foreach (var data in allData)
                {
                    if (!IntervalHelper.CheckIntervalEquivalence(peerInfo.PredecessorPeer.Id, data.Id, peerInfo.Peer.Id, peerInfo.DimensionFingerTable))
                    {
                        using (var chordClient = _peerClientFactory.Build<ChordClient>(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
                        {
                            await chordClient.AddKey(data.Id, data.Value, timeoutMS: _options.RequestExpirationTimeMS, cancellationToken: _cancellationTokenSource.Token);
                            _peerDataStore.Remove(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
