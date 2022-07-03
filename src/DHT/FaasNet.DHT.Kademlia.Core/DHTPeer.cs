using FaasNet.DHT.Kademlia.Client;
using FaasNet.DHT.Kademlia.Client.Extensions;
using FaasNet.DHT.Kademlia.Client.Messages;
using FaasNet.DHT.Kademlia.Core.Handlers;
using FaasNet.DHT.Kademlia.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.DHT.Kademlia.Core
{
    public interface IDHTPeer
    {
        bool IsRunning { get; }
        IDHTPeerInfoStore PeerInfoStore {  get; }
        IPeerDataStore PeerDataStore { get; }
        void StartSeedPeer(long id, string url, int port, CancellationToken cancellationToken = default(CancellationToken));
        void StartPeer(long id, string url, int port, string seedUrl, int seedPort, CancellationToken cancellationToken = default(CancellationToken));
        Task Stop();
    }

    public class DHTPeer : IDHTPeer
    {
        private readonly ILogger<DHTPeer> _logger;
        private readonly DHTOptions _options;
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IPeerDataStore _peerDataStore;
        private readonly IEnumerable<IRequestHandler> _requestHandlers;
        private CancellationTokenSource _cancellationTokenSource;
        private UdpClient _server;
        private System.Timers.Timer _fixKBucketLstTimer;
        private System.Timers.Timer _healthCheckTimer;
        private bool _isSeedPeer = false;
        private string _seedUrl;
        private int _seedPort;

        public DHTPeer(ILogger<DHTPeer> logger, IOptions<DHTOptions> options, IDHTPeerInfoStore peerInfoStore, IPeerDataStore peerDataStore, IEnumerable<IRequestHandler> requestHandlers)
        {
            _logger = logger;
            _options = options.Value;
            _peerInfoStore = peerInfoStore;
            _peerDataStore = peerDataStore;
            _requestHandlers = requestHandlers;
        }

        public bool IsRunning { get; private set; }
        public IDHTPeerInfoStore PeerInfoStore => _peerInfoStore;
        public IPeerDataStore PeerDataStore => _peerDataStore;

        public void StartSeedPeer(long id, string url, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            _isSeedPeer = true;
            Start(id, url, port, cancellationToken);
        }

        public void StartPeer(long id, string url, int port, string seedUrl, int seedPort, CancellationToken cancellationToken = default(CancellationToken))
        {
            _isSeedPeer = false;
            _seedUrl = seedUrl;
            _seedPort = seedPort;
            Start(id, url, port, cancellationToken);
        }

        public async Task Stop()
        {
            if (IsRunning) throw new InvalidOperationException("The peer is not started");
            await TransferAllData();
            if (!_isSeedPeer) _fixKBucketLstTimer.Stop();
            _cancellationTokenSource?.Cancel();
            _server.Close();
            IsRunning = false;
        }

        private void StartFixKBucketLst()
        {
            _fixKBucketLstTimer.AutoReset = false;
            _fixKBucketLstTimer.Start();
        }

        private void StartHealthCheck()
        {
            _healthCheckTimer.AutoReset = false;
            _healthCheckTimer.Start();
        }

        private void Start(long id, string url, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsRunning) throw new InvalidOperationException("The peer is already started");
            var peerInfoStore = DHTPeerInfo.Create(id, url, port, _options.S);
            peerInfoStore.TryAddPeer(url, port, id);
            _peerInfoStore.Update(peerInfoStore);
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var localEdp = new IPEndPoint(IPAddress.Any, port);
            _server = new UdpClient(localEdp);
#pragma warning disable 4014
            Task.Run(async () => await InternalRun(), cancellationToken);
#pragma warning restore 4014
            _fixKBucketLstTimer = new System.Timers.Timer(_options.FixKBucketLstTimerMS);
            _fixKBucketLstTimer.Elapsed += async (o, e) => await FixKBucketList(o, e);
            _healthCheckTimer = new System.Timers.Timer(_options.HealthCheckTimerMS);
            _healthCheckTimer.Elapsed += async (o, e) => await HealthCheck(o, e);
            StartFixKBucketLst();
            StartHealthCheck();
        }

        private async Task InternalRun()
        {
            try
            {
                while (true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var receiveResult = await _server.ReceiveAsync().WithCancellation(_cancellationTokenSource.Token);
                    await HandlePackage(receiveResult);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task HandlePackage(UdpReceiveResult transportResult)
        {
            var bufferContext = new ReadBufferContext(transportResult.Buffer.ToArray());
            var package = BasePackage.Deserialize(bufferContext);
            _logger.LogInformation("Receive the request {Request}", package.Command.Name);
            var requestHandler = _requestHandlers.First(r => r.Command == package.Command);
            var packageResult = await requestHandler.Handle(package, _cancellationTokenSource.Token);
            _logger.LogInformation("Send the result {Result}", packageResult.Command.Name);
            var writeBufferContext = new WriteBufferContext();
            packageResult.Serialize(writeBufferContext);
            var payload = writeBufferContext.Buffer.ToArray();
            await _server.SendAsync(payload, payload.Length, transportResult.RemoteEndPoint).WithCancellation(_cancellationTokenSource.Token);
        }

        private async Task FixKBucketList(object source, ElapsedEventArgs e)
        {
            try
            {
                if(!_isSeedPeer)
                {
                    var peerInfo = _peerInfoStore.Get();
                    using (var client = new KademliaClient(_seedUrl, _seedPort))
                    {
                        var findResult = await client.FindNode(peerInfo.Id, peerInfo.Url, peerInfo.Port, peerInfo.Id, _cancellationTokenSource.Token);
                        foreach (var peer in findResult.Peers) peerInfo.TryAddPeer(peer.Url, peer.Port, peer.Id);
                    }

                    _peerInfoStore.Update(peerInfo);
                }

                await TryTransferDataToClosestPeers();
                StartFixKBucketLst();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                StartFixKBucketLst();
            }
        }

        private async Task HealthCheck(object source, ElapsedEventArgs e)
        {
            var peerInfo = _peerInfoStore.Get();
            var peers = peerInfo.KBucketLst.SelectMany(b => b.Peers);
            foreach(var peer in peers)
            {
                try
                {
                    using (var client = new KademliaClient(peer.Url, peer.Port))
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

            StartHealthCheck();
        }

        private async Task TryTransferDataToClosestPeers()
        {
            var peerInfo = _peerInfoStore.Get();
            var allData = _peerDataStore.GetAll();
            foreach(var data in allData)
            {
                var result = peerInfo.FindClosestPeers(data.Id, 1);
                if (!result.Any() || result.First().PeerId == peerInfo.Id) continue;
                var targetPeer = result.First();
                using (var client = new KademliaClient(targetPeer.Url, targetPeer.Port))
                    await client.StoreValue(data.Id, data.Value, _cancellationTokenSource.Token);

                _peerDataStore.TryRemove(data);
            }
        }

        private async Task TransferAllData()
        {
            var peerInfo = _peerInfoStore.Get();
            var allData = _peerDataStore.GetAll();
            foreach(var data in allData)
            {
                var result = peerInfo.FindClosestPeers(data.Id, _options.K).Where(p => p.PeerId != peerInfo.Id);
                if (!result.Any()) continue;
                var targetPeer = result.First();
                using (var client = new KademliaClient(targetPeer.Url, targetPeer.Port))
                    await client.ForceStoreValue(data.Id, data.Value, peerInfo.Id, _cancellationTokenSource.Token);
            }
        }
    }
}
