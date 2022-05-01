using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public abstract class BaseNodeHost
    {
        private readonly IPeerStore _peerStore;
        private readonly IPeerHostFactory _peerHostFactory;
        private readonly ILogger<BaseNodeHost> _logger;
        private readonly ConsensusPeerOptions _options;
        private BlockingCollection<IPeerHost> _peers;

        public BaseNodeHost(IPeerStore peerStore, IPeerHostFactory peerHostFactory, ILogger<BaseNodeHost> logger, IOptions<ConsensusPeerOptions> options)
        {
            _peerStore = peerStore;
            _peerHostFactory = peerHostFactory;
            _logger = logger;
            _options = options.Value;
        }

        public bool IsRunning { get; private set; }
        public UdpClient UdpServer { get; private set; }
        public event EventHandler<EventArgs> NodeStarted;
        public event EventHandler<EventArgs> NodeStopped;
        protected CancellationTokenSource TokenSource { get; private set; }

        public async Task Start(CancellationToken cancellationToken)
        {
            if (IsRunning) throw new InvalidOperationException("The node is already running");
            _peers = new BlockingCollection<IPeerHost>();
            TokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var peerInfoLst = await _peerStore.GetAll(cancellationToken);
            Parallel.ForEach(peerInfoLst, async (peerInfo) =>
            {
                var peerHost = _peerHostFactory.Build();
                await peerHost.Start(peerInfo, cancellationToken);
                _peers.Add(peerHost);
            });
            UdpServer = BuildUdpClient();
#pragma warning disable 4014
            Task.Run(async () => await InternalRun(), cancellationToken);
#pragma warning restore 4014
            IsRunning = true;
        }

        public Task Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The node is not running");
            Parallel.ForEach(_peers, async (peer) =>
            {
                await peer.Stop();
            });
            IsRunning = false;
            UdpServer.Close();
            return Task.CompletedTask;
        }

        protected async Task InternalRun()
        {
            if (NodeStarted != null) NodeStarted(this, new EventArgs());
            try
            {
                while(true)
                {
                    TokenSource.Token.ThrowIfCancellationRequested();
                    await HandleUDPPackage();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            if (NodeStopped != null) NodeStopped(this, new EventArgs());
        }

        private async Task HandleUDPPackage()
        {
            try
            {
                var udpResult = await UdpServer.ReceiveAsync().WithCancellation(TokenSource.Token);
                var bufferContext = new ReadBufferContext(udpResult.Buffer);
                var consensusPackage = ConsensusPackage.Deserialize(bufferContext);
                // if(consensusPackage != null) 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private UdpClient BuildUdpClient()
        {
            return new UdpClient(new IPEndPoint(IPAddress.Any, _options.Port));
        }
    }
}
