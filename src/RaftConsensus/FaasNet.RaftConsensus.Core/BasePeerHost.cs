using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
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
        PeerInfo Info { get; }
        Task Start(string nodeId, PeerInfo info, CancellationToken cancellationToken);
        Task Stop();
    }

    public abstract class BasePeerHost : IPeerHost
    {
        private readonly ILogger<BasePeerHost> _logger;
        private readonly ConsensusPeerOptions _options;
        private readonly IClusterStore _clusterStore;
        private DateTime? _lastLeaderHeartbeatReceivedDateTime = null;
        private DateTime? _expirationCheckElectionDateTime = null;
        private int _nbPositiveVote = 0;
        private int _quorum = 0;
        private string _nodeId;
        private System.Timers.Timer _checkLeaderHeartbeatTimer;
        private System.Timers.Timer _electionCheckTimer;
        private System.Timers.Timer _leaderHeartbeatTimer;

        public BasePeerHost(ILogger<BasePeerHost> logger, IOptions<ConsensusPeerOptions> options, IClusterStore clusterStore)
        {
            _logger = logger;
            _options = options.Value;
            _clusterStore = clusterStore;
        }

        public bool IsRunning { get; private set; }
        public PeerInfo Info { get; private set; }
        public PeerStates State { get; private set; }
        public CancellationTokenSource TokenSource { get; private set; }
        public UdpClient UdpServer { get; private set; }
        public IPEndPoint UdpServerEdp { get; private set; }
        public event EventHandler<EventArgs> PeerHostStarted;
        public event EventHandler<EventArgs> PeerHostStopped;

        public Task Start(string nodeId, PeerInfo info, CancellationToken cancellationToken)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (IsRunning) throw new InvalidOperationException("The peer is already running");
            _logger.LogInformation("Start peer");
            TokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            SetFollower();
            _nodeId = nodeId;
            UdpServer = BuildUdpClient();
            _checkLeaderHeartbeatTimer = new System.Timers.Timer();
            _electionCheckTimer = new System.Timers.Timer(_options.CheckElectionTimerMS);
            _leaderHeartbeatTimer = new System.Timers.Timer(_options.LeaderHeartbeatTimerMS);
            _checkLeaderHeartbeatTimer.Elapsed += async(o, e) => await CheckLeaderHeartbeat(o, e);
            _electionCheckTimer.Elapsed += async (o, e) => await CheckElection(o, e);
            _leaderHeartbeatTimer.Elapsed += async (o, e) => await BroadcastLeaderHeartbeat(o, e);
            _electionCheckTimer.AutoReset = false;
            _checkLeaderHeartbeatTimer.AutoReset = false;
            _leaderHeartbeatTimer.AutoReset = false;
            StartCheckLeaderHeartbeat();
#pragma warning disable 4014
            Task.Run(async () => await InternalRun(), cancellationToken);
#pragma warning restore 4014
            IsRunning = true;
            Info = info;
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The peer is not running");
            _logger.LogInformation("Stop peer");
            TokenSource.Cancel();
            UdpServer.Close();
            StopCheckLeaderHeartbeat();
            StopCheckElection();
            IsRunning = false;
            return Task.CompletedTask;
        }

        #region Check learder heartbeat

        private async Task CheckLeaderHeartbeat(object source, ElapsedEventArgs e)
        {
            if (State != PeerStates.FOLLOWER) return;
            var currentDatTime = DateTime.UtcNow;
            if (_lastLeaderHeartbeatReceivedDateTime != null && _lastLeaderHeartbeatReceivedDateTime.Value.AddMilliseconds(_options.LeaderHeartbeatDurationMS) >= currentDatTime)
            {
                _logger.LogInformation("{Node}:{TermId}, Leader heartbeat received {receptionDateTime}", _nodeId, Info.TermId, _lastLeaderHeartbeatReceivedDateTime);
                StartCheckLeaderHeartbeat();
                return;
            }

            await StartElection();
        }

        private void StopCheckLeaderHeartbeat()
        {
            _checkLeaderHeartbeatTimer?.Stop();
        }

        private void StartCheckLeaderHeartbeat()
        {
            if(_checkLeaderHeartbeatTimer != null) _checkLeaderHeartbeatTimer.Interval = new Random().Next(1000, 3000);
            _checkLeaderHeartbeatTimer?.Start();
        }

        private async Task StartElection()
        {
            // Start to vote.
            _logger.LogInformation("{Node}:{TermId}, Start to vote", _nodeId, Info.TermId);
            var nodes = await _clusterStore.GetAllNodes(TokenSource.Token);
            _quorum = (nodes.Count() / 2) + 1;
            if (_quorum == 0)
            {
                _logger.LogError("{Node}:{TermId}, There is no enough nodes", _nodeId, Info.TermId);
                return;
            }

            _nbPositiveVote = 0;
            _expirationCheckElectionDateTime = DateTime.UtcNow.AddMilliseconds(_options.ElectionCheckDurationMS);
            SetCandidate();
            var pkg = PackageRequestBuilder.Vote(Info.TermId, Info.TermIndex + 1);
            foreach (var peer in nodes)
            {
                var edp = new IPEndPoint(ResolveIPAddress(peer.Url), peer.Port);
                await Send(pkg, edp, TokenSource.Token);
            }
        }

        #endregion

        #region Check election

        private Task CheckElection(object source, ElapsedEventArgs e)
        {
            if (State != PeerStates.CANDIDATE || _expirationCheckElectionDateTime == null) return Task.CompletedTask;
            var currentDateTime = DateTime.UtcNow;
            if (_nbPositiveVote >= _quorum)
            {
                _logger.LogInformation("{Node}:{TermId}, Peer is now the leader !", _nodeId, Info.TermId);
                StartBroadcastLeaderHeartbeat();
                SetLeader();
                Info.TermIndex++;
                return Task.CompletedTask;
            }

            if (_expirationCheckElectionDateTime.Value <= currentDateTime)
            {
                _logger.LogInformation("{Node}:{TermId}, Finish to check the election", _nodeId, Info.TermId);
                SetFollower();
                return Task.CompletedTask;
            }

            StartCheckElection();
            return Task.CompletedTask;
        }

        private void StartCheckElection()
        {
            _electionCheckTimer?.Start();
        }

        private void StopCheckElection()
        {
            _electionCheckTimer?.Stop();
        }

        #endregion

        #region Broadcast heartbeat

        private async Task BroadcastLeaderHeartbeat(object source, ElapsedEventArgs e)
        {
            if (State != PeerStates.LEADER) return;
            var pkg = PackageRequestBuilder.LeaderHeartbeat(Info.TermId, Info.TermIndex);
            var nodes = await _clusterStore.GetAllNodes(TokenSource.Token);
            foreach (var peer in nodes)
            {
                var edp = new IPEndPoint(ResolveIPAddress(peer.Url), peer.Port);
                await Send(pkg, edp, TokenSource.Token);
            }

            StartBroadcastLeaderHeartbeat();
        }

        private void StartBroadcastLeaderHeartbeat()
        {
            _leaderHeartbeatTimer?.Start();
        }

        private void StopBroadcastLeaderHeartbeat()
        {
            _leaderHeartbeatTimer?.Stop();
        }

        #endregion

        #region Change status

        private void SetFollower()
        {
            State = PeerStates.FOLLOWER;
            StartCheckLeaderHeartbeat();
            StopCheckElection();
            StopBroadcastLeaderHeartbeat();
        }

        private void SetCandidate()
        {
            State = PeerStates.CANDIDATE;
            StopCheckLeaderHeartbeat();
            StartCheckElection();
            StopBroadcastLeaderHeartbeat();
        }

        private void SetLeader()
        {
            State = PeerStates.LEADER;
            StopCheckLeaderHeartbeat();
            StopCheckElection();
            StartBroadcastLeaderHeartbeat();
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
                ConsensusPackage result = null;
                if (consensusPackage == null) return false;
                if (consensusPackage.Header.Command == ConsensusCommands.LEADER_HEARTBEAT_REQUEST) result = await Handle(consensusPackage as LeaderHeartbeatRequest, cancellationToken);
                if (consensusPackage.Header.Command == ConsensusCommands.VOTE_REQUEST) result = await Handle(consensusPackage as VoteRequest, cancellationToken);
                if (consensusPackage.Header.Command == ConsensusCommands.VOTE_RESULT) result = await Handle(consensusPackage as VoteResult, cancellationToken);
                result = result == null ? PackageResultBuilder.Empty(Info.TermId, Info.TermIndex) : result;
                await Send(result, udpResult.RemoteEndPoint, cancellationToken);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        private Task<ConsensusPackage> Handle(LeaderHeartbeatRequest consensusPackage, CancellationToken cancellationToken)
        {
            if (consensusPackage.Header.TermId != Info.TermId) return null;
            _logger.LogInformation("{Node}:{TermId}, Heartbeat is received", _nodeId, Info.TermId);
            _lastLeaderHeartbeatReceivedDateTime = DateTime.UtcNow;
            return Task.FromResult((ConsensusPackage)null);
        }

        private Task<ConsensusPackage> Handle(VoteRequest voteRequest, CancellationToken cancellationToken)
        {
            var isGranted = true;
            if (Info.TermId != voteRequest.Header.TermId) isGranted = false;
            else
            {
                if (voteRequest.Header.TermIndex > Info.TermIndex)
                {
                    SetFollower();
                } 
                else if (
                    (State == PeerStates.CANDIDATE || State == PeerStates.LEADER) ||
                    (Info.TermIndex >= voteRequest.Header.TermIndex)
                )
                {
                    isGranted = false;
                }
            }

            return Task.FromResult(PackageResultBuilder.Vote(Info.TermId, Info.TermIndex, isGranted));
        }

        private Task<ConsensusPackage> Handle(VoteResult voteResult, CancellationToken cancellationToken)
        {
            if (voteResult.VoteGranted) _nbPositiveVote++;
            return Task.FromResult((ConsensusPackage)null);
        }

        #endregion

        private UdpClient BuildUdpClient()
        {
            var localEdp = new IPEndPoint(IPAddress.Any, 0);
            var result = new UdpClient(localEdp);
            UdpServerEdp = new IPEndPoint(IPAddress.Loopback, ((IPEndPoint)(result.Client.LocalEndPoint)).Port);
            return result;
        }

        private async Task Send(ConsensusPackage pkg, IPEndPoint ipEdp, CancellationToken cancellationToken)
        {
            var writeCtx = new WriteBufferContext();
            pkg.Serialize(writeCtx);
            var resultPayload = writeCtx.Buffer.ToArray();
            await UdpServer.SendAsync(resultPayload, resultPayload.Count(), ipEdp).WithCancellation(cancellationToken);
        }

        private static IPAddress ResolveIPAddress(string url)
        {
            var hostEntry = Dns.GetHostEntry(url);
            return hostEntry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
