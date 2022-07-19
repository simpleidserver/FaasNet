using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer
{
    public interface IPeerHost
    {
        Task Start(CancellationToken cancellationToken = default(CancellationToken));
        Task Stop();
    }

    public class PeerHost : IPeerHost
    {
        private readonly ITransport _transport;
        private readonly IProtocolHandlerFactory _protocolHandlerFactory;
        private readonly IClusterStore _clusterStore;
        private readonly IEnumerable<ITimer> _timers;
        private readonly ILogger<PeerHost> _logger;
        private readonly PeerOptions _options;
        private CancellationTokenSource _cancellationTokenSource;

        public PeerHost(ITransport transport, IProtocolHandlerFactory protocolHandlerFactory, IClusterStore clusterStore, IEnumerable<ITimer> timers, ILogger<PeerHost> logger, IOptions<PeerOptions> options)
        {
            _transport = transport;
            _protocolHandlerFactory = protocolHandlerFactory;
            _clusterStore = clusterStore;
            _timers = timers;
            _logger = logger;
            _options = options.Value;
        }

        public bool IsRunning { get; private set; }

        public async Task Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsRunning) throw new InvalidOperationException("The Peer is already running");
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await _clusterStore.SelfRegister(new ClusterPeer(_options.Url, _options.Port), cancellationToken);
            _transport.Start(_cancellationTokenSource.Token);
#pragma warning disable 4014
            Task.Run(async () => await Run(), cancellationToken);
#pragma warning restore 4014
            IsRunning = true;
            foreach (var timer in _timers) await timer.Start(_cancellationTokenSource.Token);
        }

        public Task Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The Peer is not running");
            foreach (var timer in _timers) timer.Stop();
            _cancellationTokenSource.Cancel();
            _transport.Stop();
            return Task.CompletedTask;
        }

        protected async Task Run()
        {
            try
            {
                while(true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var sessionResult = await _transport.ReceiveMessage();
                    var payload = sessionResult.Payload.ToArray();
                    var readBufferContext = new ReadBufferContext(payload);
                    string magicCode = readBufferContext.NextString(), version = readBufferContext.NextString();
                    var protocolHandler = _protocolHandlerFactory.Build(magicCode);
                    var result = await protocolHandler.Handle(readBufferContext.Buffer.ToArray(), _cancellationTokenSource.Token);
                    var ctx = new WriteBufferContext();
                    result.SerializeEnvelope(ctx);
                    await sessionResult.ResponseCallback(ctx.Buffer.ToArray());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
