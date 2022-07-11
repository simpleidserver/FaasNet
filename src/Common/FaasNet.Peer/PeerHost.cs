using FaasNet.Peer.Transports;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer
{
    public class PeerHost
    {
        private readonly ITransport _transport;
        private readonly ILogger<PeerHost> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public PeerHost(ITransport transport, ILogger<PeerHost> logger)
        {
            _transport = transport;
            _logger = logger;
        }

        public bool IsRunning { get; private set; }

        public async Task Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsRunning) throw new InvalidOperationException("The Peer is already running");
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _transport.Start(_cancellationTokenSource.Token);
#pragma warning disable 4014
            Task.Run(async () => await Run(), cancellationToken);
#pragma warning restore 4014
            IsRunning = true;
        }

        public Task Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The Peer is not running");
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

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
