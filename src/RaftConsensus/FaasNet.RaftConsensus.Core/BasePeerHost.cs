using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.RaftConsensus.Core
{
    public interface IPeerHost
    {
        IPEndPoint UdpServerEdp { get; }
        Task Start(PeerInfo info, CancellationToken cancellationToken);
        Task Stop();
    }

    public abstract class BasePeerHost : IPeerHost
    {
        private readonly ILogger<BasePeerHost> _logger;
        private readonly ConsensusPeerOptions _options;
        private readonly IClusterStore _clusterNode;
        private DateTime? _lastLeaderHeartbeatReceivedDateTime = null;
        private System.Timers.Timer _leaderHeartbeatTimer;

        public BasePeerHost(ILogger<BasePeerHost> logger, IOptions<ConsensusPeerOptions> options, IClusterStore clusterStore)
        {
            _logger = logger;
            _options = options.Value;
            _clusterNode = clusterStore;
        }

        public bool IsRunning { get; private set; }
        public PeerInfo Info { get; private set; }
        public PeerStates State { get; private set; }
        public CancellationTokenSource TokenSource { get; private set; }
        public UdpClient UdpServer { get; private set; }
        public IPEndPoint UdpServerEdp { get; private set; }
        public event EventHandler<EventArgs> PeerHostStarted;
        public event EventHandler<EventArgs> PeerHostStopped;

        public Task Start(PeerInfo info, CancellationToken cancellationToken)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (IsRunning) throw new InvalidOperationException("The peer is already running");
            _logger.LogInformation("Start peer");
            TokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            State = PeerStates.FOLLOWER;
            UdpServer = BuildUdpClient();
            _leaderHeartbeatTimer = new System.Timers.Timer(_options.CheckLeaderHeartbeatTimerMS)
            {
                Enabled = true
            };
            _leaderHeartbeatTimer.Elapsed += async(o, e) => await CheckLeaderHeartbeat(o, e);
#pragma warning disable 4014
            Task.Run(async () => await InternalRun(), cancellationToken);
#pragma warning restore 4014
            IsRunning = true;
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The peer is not running");
            _logger.LogInformation("Stop peer");
            TokenSource.Cancel();
            UdpServer.Close();
            _leaderHeartbeatTimer.Stop();
            IsRunning = false;
            return Task.CompletedTask;
        }

        #region Timers

        private Task CheckLeaderHeartbeat(object source, ElapsedEventArgs e)
        {
            if (State != PeerStates.FOLLOWER) return Task.CompletedTask;
            var currentDatTime = DateTime.UtcNow;
            if (_lastLeaderHeartbeatReceivedDateTime == null || _lastLeaderHeartbeatReceivedDateTime.Value.AddMilliseconds(_options.LeaderHeartbeatDurationMS) >= currentDatTime)
            {
                return Task.CompletedTask;
            }

            State = PeerStates.CANDIDATE;
            // TODO: Send election process.
            return Task.CompletedTask;
        }

        #endregion

        #region TCP Packages

        protected async Task InternalRun()
        {
            if (PeerHostStarted != null) PeerHostStarted(this, new EventArgs());
            await Init(TokenSource.Token);
            try
            {
                while (true)
                {
                    TokenSource.Token.ThrowIfCancellationRequested();
                    await HandleUDPPackage();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            if (PeerHostStopped != null) PeerHostStopped(this, new EventArgs());
        }

        private async Task HandleUDPPackage()
        {
            try
            {
                while(true)
                {
                    TokenSource.Token.ThrowIfCancellationRequested();
                    var receiveResult = await UdpServer.ReceiveAsync().WithCancellation(TokenSource.Token);
                    if (await HandleRaftConsensusPackage(receiveResult, TokenSource.Token)) continue;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        protected abstract Task Init(CancellationToken token);
        protected abstract Task<bool> HandlePackage(UdpReceiveResult udpResult);

        private async Task<bool> HandleRaftConsensusPackage(UdpReceiveResult udpResult, CancellationToken cancellationToken)
        {
            try
            {
                var bufferContext = new ReadBufferContext(udpResult.Buffer);
                var consensusPackage = ConsensusPackage.Deserialize(bufferContext);
                if (consensusPackage == null) return false;
                if (consensusPackage.Header.Command == ConsensusCommands.LEADER_HEARTBEAT_REQUEST) await Handle(consensusPackage as LeaderHeartbeatRequest, cancellationToken);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        private async Task Handle(LeaderHeartbeatRequest consensusPackage, CancellationToken cancellationToken)
        {
            if (consensusPackage.Header.TermId != Info.TermId) return;
            _lastLeaderHeartbeatReceivedDateTime = DateTime.UtcNow;
        }

        #endregion

        private UdpClient BuildUdpClient()
        {
            UdpServerEdp = new IPEndPoint(IPAddress.Any, 0);
            return new UdpClient(UdpServerEdp);
        }
    }
}
