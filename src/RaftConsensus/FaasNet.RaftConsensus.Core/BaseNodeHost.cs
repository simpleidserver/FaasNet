using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public interface INodeHost
    {
        string NodeId { get; }
        BlockingCollection<IPeerHost> Peers { get; }
        Task Start(CancellationToken cancellationToken);
        Task Stop();
    }

    public abstract class BaseNodeHost : INodeHost
    {
        private readonly IPeerStore _peerStore;
        private readonly IPeerHostFactory _peerHostFactory;
        private readonly ConsensusPeerOptions _options;
        private readonly UdpClient _proxyClient;
        private BlockingCollection<IPeerHost> _peers;
        private string _nodeId;

        public BaseNodeHost(IPeerStore peerStore, IPeerHostFactory peerHostFactory, ILogger<BaseNodeHost> logger, IOptions<ConsensusPeerOptions> options)
        {
            _peerStore = peerStore;
            _peerHostFactory = peerHostFactory;
            Logger = logger;
            _options = options.Value;
            _proxyClient = new UdpClient();
            _nodeId = Guid.NewGuid().ToString();
        }

        public BlockingCollection<IPeerHost> Peers => _peers;
        public bool IsRunning { get; private set; }
        public UdpClient UdpServer { get; private set; }
        public ILogger<BaseNodeHost> Logger { get; private set; }
        public string NodeId => _nodeId;
        public event EventHandler<EventArgs> NodeStarted;
        public event EventHandler<EventArgs> NodeStopped;
        protected CancellationTokenSource TokenSource { get; private set; }

        public async Task Start(CancellationToken cancellationToken)
        {
            if (IsRunning) throw new InvalidOperationException("The node is already running");
            _peers = new BlockingCollection<IPeerHost>();
            TokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var peerInfoLst = await _peerStore.GetAll(cancellationToken);
            foreach(var peerInfo in peerInfoLst)
            {
                var peerHost = _peerHostFactory.Build();
                await peerHost.Start(_nodeId, peerInfo, cancellationToken);
                _peers.Add(peerHost);
            }

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
            _proxyClient.Close();
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
                Logger.LogError(ex.ToString());
            }

            if (NodeStopped != null) NodeStopped(this, new EventArgs());
        }

        protected async Task HandleUDPPackage()
        {
            try
            {
                var udpResult = await UdpServer.ReceiveAsync().WithCancellation(TokenSource.Token);
                var bufferContext = new ReadBufferContext(udpResult.Buffer.ToArray());
                var consensusPackage = ConsensusPackage.Deserialize(bufferContext);
                if (consensusPackage == null) { await HandleUDPPackage(udpResult, TokenSource.Token); return; }
                var peerHost = _peers.First(p => p.Info.TermId == consensusPackage.Header.TermId);
                await _proxyClient.SendAsync(udpResult.Buffer, udpResult.Buffer.Count(), peerHost.UdpServerEdp);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        protected abstract Task HandleUDPPackage(UdpReceiveResult udpResult, CancellationToken cancellationToken);

        private UdpClient BuildUdpClient()
        {
            return new UdpClient(new IPEndPoint(IPAddress.Any, _options.Port));
        }
    }
}
