using FaasNet.Peer.Client;
using FaasNet.Peer.Transports;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer
{
    public interface IPeerHost
    {
        Task Start(CancellationToken cancellationToken = default(CancellationToken));
        Task Stop();
    }

    public abstract class BasePeerHost : IPeerHost
    {
        private readonly IServerTransport _transport;
        private readonly IProtocolHandlerFactory _protocolHandlerFactory;
        private readonly IEnumerable<ITimer> _timers;
        private readonly ILogger<BasePeerHost> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public BasePeerHost(IServerTransport transport, IProtocolHandlerFactory protocolHandlerFactory, IEnumerable<ITimer> timers, ILogger<BasePeerHost> logger)
        {
            _transport = transport;
            _protocolHandlerFactory = protocolHandlerFactory;
            _timers = timers;
            _logger = logger;
        }

        public bool IsRunning { get; private set; }

        public async Task Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsRunning) throw new InvalidOperationException("The Peer is already running");
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await Init(cancellationToken);
            _transport.Start(_cancellationTokenSource.Token);
#pragma warning disable 4014
            Task.Run(async () => await Run(), cancellationToken);
#pragma warning restore 4014
            IsRunning = true;
            foreach (var timer in _timers) await timer.Start(_cancellationTokenSource.Token);
        }

        protected CancellationTokenSource TokenSource => _cancellationTokenSource;

        public Task Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The Peer is not running");
            foreach (var timer in _timers) timer.Stop();
            _cancellationTokenSource.Cancel();
            _transport.Stop();
            return Task.CompletedTask;
        }

        protected abstract Task Init(CancellationToken cancellationToken = default(CancellationToken));

        protected async Task Run()
        {
            try
            {
                while (true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var sessionResult = await _transport.ReceiveSession();
#pragma warning disable CS4014
                    Task.Run(() => ReceiveMessage(sessionResult));
#pragma warning restore CS4014 
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        protected virtual async Task ReceiveMessage(BaseSessionResult session)
        {
            var payload = await session.ReceiveMessage();
            if (payload.Length == 0) return;
            var readBufferContext = new ReadBufferContext(payload);
            string magicCode = readBufferContext.NextString(), version = readBufferContext.NextString();
            var protocolHandler = _protocolHandlerFactory.Build(magicCode);
            var result = await protocolHandler.Handle(readBufferContext.Buffer.ToArray(), _cancellationTokenSource.Token);
            var ctx = new WriteBufferContext();
            result.SerializeEnvelope(ctx);
            await session.SendMessage(ctx.Buffer.ToArray());
        }
    }
}
