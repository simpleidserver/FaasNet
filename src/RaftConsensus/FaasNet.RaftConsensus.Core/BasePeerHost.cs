using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.RaftConsensus.Core
{
    public abstract class BasePeerHost
    {
        private readonly ILogger<BasePeerHost> _logger;
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly ConsensusPeerOptions _options;
        private readonly IClusterStore _clusterNode;
        private readonly ITermInfoStore _termInfoStore;
        private DateTime? _lastLeaderHeartbeatReceivedDateTime = null;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private UdpClient _udpClient;
        private System.Timers.Timer _leaderHeartbeatTimer;

        public BasePeerHost(ILogger<BasePeerHost> logger, IUdpClientServerFactory udpClientServerFactory, IOptions<ConsensusPeerOptions> options, IClusterStore clusterStore, ITermInfoStore termInfoStore)
        {
            _logger = logger;
            _udpClientFactory = udpClientServerFactory;
            _options = options.Value;
            _clusterNode = clusterStore;
            _termInfoStore = termInfoStore;
        }

        public bool IsRunning { get; private set; }
        public event EventHandler<EventArgs> PeerHostStarted;

        public async Task Run(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start peer");
            await _termInfoStore.ResetFollower(cancellationToken);
            await _termInfoStore.SaveChanges(cancellationToken);
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
            _udpClient = _udpClientFactory.Build();
            _leaderHeartbeatTimer = new System.Timers.Timer(_options.CheckLeaderHeartbeatTimerMS)
            {
                Enabled = true
            };
            _leaderHeartbeatTimer.Elapsed += async(o, e) => await CheckLeaderHeartbeat(o, e);
            IsRunning = true;
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop peer");
            _tokenSource.Cancel();
            _udpClient.Close();
            _leaderHeartbeatTimer.Stop();
            IsRunning = false;
        }

        #region Timers

        private async Task CheckLeaderHeartbeat(object source, ElapsedEventArgs e)
        {
            var termInfos = await _termInfoStore.GetAll(_cancellationToken);
            var peers = await _clusterNode.GetAllPeers(CancellationToken.None);
            foreach (var termInfo in termInfos)
            {
                if (termInfo.State != PeerStates.FOLLOWER) continue;
                var currentDatTime = DateTime.UtcNow;
                if (_lastLeaderHeartbeatReceivedDateTime == null || _lastLeaderHeartbeatReceivedDateTime.Value.AddMilliseconds(_options.LeaderHeartbeatDurationMS) >= currentDatTime)
                {
                    continue;
                }

                termInfo.State = PeerStates.CANDIDATE;
            }

            await _termInfoStore.SaveChanges(_cancellationToken);
            // Chaque terme correspond à une queue.
        }

        #endregion

        #region TCP Packages

        private async Task HandleUDPPackage()
        {
            if (PeerHostStarted != null)
            {
                PeerHostStarted(this, new EventArgs());
            }

            try
            {
                while(true)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    var receiveResult = await _udpClient.ReceiveAsync().WithCancellation(_cancellationToken);
                    if (await HandleRaftConsensusPackage(receiveResult, _cancellationToken)) continue;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

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
            var termInfo = await _termInfoStore.Get(consensusPackage.TermId, cancellationToken);
            if(termInfo == null)
            {
                return;
            }

            if(termInfo.Index != consensusPackage.Index)
            {
                return;
            }

            termInfo.LastHeartbeatRequest = DateTime.UtcNow;
            await _termInfoStore.SaveChanges(cancellationToken);
        }

        #endregion
    }
}
