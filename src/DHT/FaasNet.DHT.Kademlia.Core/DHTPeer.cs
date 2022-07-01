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

namespace FaasNet.DHT.Kademlia.Core
{
    public interface IDHTPeer
    {
        bool IsRunning { get; }
        void Start(long id, string url, int port, CancellationToken cancellationToken = default(CancellationToken));
        void Stop();
    }

    public class DHTPeer : IDHTPeer
    {
        private readonly ILogger<DHTPeer> _logger;
        private readonly DHTOptions _options;
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private readonly IEnumerable<IRequestHandler> _requestHandlers;
        private CancellationTokenSource _cancellationTokenSource;
        private UdpClient _server;

        public DHTPeer(ILogger<DHTPeer> logger, IOptions<DHTOptions> options, IDHTPeerInfoStore peerInfoStore, IEnumerable<IRequestHandler> requestHandlers)
        {
            _logger = logger;
            _options = options.Value;
            _peerInfoStore = peerInfoStore;
            _requestHandlers = requestHandlers;
        }

        public bool IsRunning { get; private set; }

        public void Start(long id, string url, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            // https://kelseyc18.github.io/kademlia_vis/basics/3/
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
        }

        public void Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The peer is not started");
            _cancellationTokenSource?.Cancel();
            _server.Close();
            IsRunning = false;
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
    }
}
