﻿using FaasNet.Common.Extensions;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Client.Messages.Consensus;
using FaasNet.RaftConsensus.Core.Models;
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
using System.Timers;

namespace FaasNet.RaftConsensus.Core
{
    public interface IPeerHost
    {
        LeaderNode ActiveNode { get; }
        IPEndPoint UdpServerEdp { get; }
        string PeerId { get; }
        PeerInfo Info { get; }
        PeerStates State { get; }
        Task Start(UdpClient udpClient, string nodeId, PeerInfo info, CancellationToken cancellationToken);
        Task Stop();
        Task AppendEntry(LogRecord logRecord, bool forceAdd, CancellationToken cancellationToken);
        Task<LogRecord> ReadRecord(int evtOffet, CancellationToken cancellationToken);
        event EventHandler<EventArgs> PeerHostStarted;
        event EventHandler<EventArgs> PeerHostStopped;
        event EventHandler<PeerHostEventArgs> PeerIsFollower;
        event EventHandler<PeerHostEventArgs> PeerIsCandidate;
        event EventHandler<PeerHostEventArgs> PeerIsLeader;
    }

    public abstract class BasePeerHost : IPeerHost
    {
        private readonly ILogger<BasePeerHost> _logger;
        private readonly ConsensusNodeOptions _nodeOptions;
        private readonly ConsensusPeerOptions _options;
        private readonly IClusterStore _clusterStore;
        private readonly ILogStore _logStore;
        private readonly IPeerInfoStore _peerStore;
        private UdpClient _udpClient;
        private readonly string _peerId;
        private ConcurrentBag<AppendEntryRequest> _appendEntryRequestLst;
        private LeaderNode _activeLeader = null;
        private DateTime? _expirationCheckElectionDateTime = null;
        private int _nbPositiveVote = 0;
        private int _quorum = 0;
        private string _nodeId;
        private System.Timers.Timer _checkLeaderHeartbeatTimer;
        private System.Timers.Timer _electionCheckTimer;
        private System.Timers.Timer _leaderHeartbeatTimer;

        public BasePeerHost(ILogger<BasePeerHost> logger, IOptions<ConsensusNodeOptions> nodeOptions, IOptions<ConsensusPeerOptions> options, IClusterStore clusterStore, ILogStore logStore, IPeerInfoStore peerStore)
        {
            _logger = logger;
            _nodeOptions = nodeOptions.Value;
            _options = options.Value;
            _clusterStore = clusterStore;
            _peerStore = peerStore;
            _logStore = logStore;
            _peerId = Guid.NewGuid().ToString();
        }

        public bool IsRunning { get; private set; }
        public string PeerId => _peerId;
        public PeerInfo Info { get; private set; }
        public PeerStates State { get; private set; }
        public CancellationTokenSource TokenSource { get; private set; }
        public UdpClient UdpServer { get; private set; }
        public IPEndPoint UdpServerEdp { get; private set; }
        public LeaderNode ActiveNode => _activeLeader;
        public event EventHandler<EventArgs> PeerHostStarted;
        public event EventHandler<EventArgs> PeerHostStopped;
        public event EventHandler<PeerHostEventArgs> PeerIsFollower;
        public event EventHandler<PeerHostEventArgs> PeerIsCandidate;
        public event EventHandler<PeerHostEventArgs> PeerIsLeader;
        protected IClusterStore ClusterStore => _clusterStore;
        protected ConsensusPeerOptions Options => _options;
        protected ILogger<BasePeerHost> Logger => _logger;

        public Task Start(UdpClient udpClient, string nodeId, PeerInfo info, CancellationToken cancellationToken)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (IsRunning) throw new InvalidOperationException("The peer is already running");
            _udpClient = udpClient;
            _logStore.TermId = info.TermId;
            _appendEntryRequestLst = new ConcurrentBag<AppendEntryRequest>();
            _logger.LogInformation("Start peer {PeerId}", _peerId);
            TokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            SetFollower();
            _nodeId = nodeId;
            UdpServer = BuildUdpServer();
            _checkLeaderHeartbeatTimer = new System.Timers.Timer(_options.CheckLeaderHeartbeatTimerMS);
            _electionCheckTimer = new System.Timers.Timer(_options.CheckElectionTimerMS);
            _leaderHeartbeatTimer = new System.Timers.Timer(_options.LeaderHeartbeatTimerMS);
            _checkLeaderHeartbeatTimer.Elapsed += async(o, e) => await CheckLeaderHeartbeat(o, e);
            _electionCheckTimer.Elapsed += async (o, e) => await CheckElection(o, e);
            _leaderHeartbeatTimer.Elapsed += async (o, e) => await BroadcastLeaderHeartbeat(o, e);
            _electionCheckTimer.AutoReset = false;
            _checkLeaderHeartbeatTimer.AutoReset = false;
            _leaderHeartbeatTimer.AutoReset = false;
            StartCheckLeaderHeartbeat(true);
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
            _logger.LogInformation("Stop peer {PeerId}", _peerId);
            TokenSource.Cancel();
            UdpServer.Close();
            StopCheckLeaderHeartbeat();
            StopCheckElection();
            IsRunning = false;
            return Task.CompletedTask;
        }

        public async Task AppendEntry(LogRecord logRecord, bool forceAdd, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Node}:{PeerId}:{TermId}:{State}, Add log", _nodeId, _peerId, Info.TermId, State);
            _logStore.Add(logRecord);
            Info.Upgrade();
            _peerStore.Update(Info);
            await AddEntry(logRecord, forceAdd, cancellationToken);
        }

        public Task<LogRecord> ReadRecord(int evtOffset, CancellationToken cancellationToken)
        {
            return _logStore.Get(evtOffset, cancellationToken);
        }

        protected abstract Task AddEntry(LogRecord logRecord, bool forceAdd, CancellationToken cancellationToken);

        #region Check learder heartbeat

        private async Task CheckLeaderHeartbeat(object source, ElapsedEventArgs e)
        {
            if (State != PeerStates.FOLLOWER) return;
            if (DateTime.UtcNow < _expirationCheckElectionDateTime)
            {
                StartCheckLeaderHeartbeat(false);
                return;
            }

            if (_activeLeader != null && _activeLeader.IsActive(_options.LeaderHeartbeatExpirationDurationMS))
            {
                if (State == PeerStates.CANDIDATE) SetFollower();
                StartCheckLeaderHeartbeat(false);
                return;
            }

            await StartElection();
        }

        private void StopCheckLeaderHeartbeat()
        {
            _checkLeaderHeartbeatTimer?.Stop();
        }

        private void StartCheckLeaderHeartbeat(bool computeExpirationTime)
        {
            if (computeExpirationTime) _expirationCheckElectionDateTime = DateTime.UtcNow.AddMilliseconds(_options.CheckLeaderHeartbeatIntervalMS.GetValue());
            _checkLeaderHeartbeatTimer?.Start();
        }

        private async Task StartElection()
        {
            var currentDateTime = DateTime.UtcNow;
            if (_expirationCheckElectionDateTime != null && _expirationCheckElectionDateTime.Value > currentDateTime) return;
            // Start to vote.
            _logger.LogInformation("{Node}:{PeerId}:{TermId}, Start to vote", _nodeId, _peerId, Info.TermId);
            var nodes = await _clusterStore.GetAllNodes(TokenSource.Token);
            nodes = nodes.Where(n => n.Port != _nodeOptions.Port || n.Url != _nodeOptions.Url);
            _quorum = !nodes.Any() ? 0 : (nodes.Count() / 2) + 1;
            /*
            if (_quorum == 0)
            {
                _logger.LogError("{Node}:{PeerId}:{TermId}, There is no enough nodes", _nodeId, _peerId, Info.TermId);
                return;
            }
            */

            _nbPositiveVote = 0;
            _expirationCheckElectionDateTime = DateTime.UtcNow.AddMilliseconds(_options.ElectionCheckDurationMS);
            SetCandidate();
            Info.Increment();
            var pkg = ConsensusPackageRequestBuilder.Vote(_nodeOptions.Url, _nodeOptions.Port, Info.TermId, Info.TermIndex);
            foreach (var peer in nodes)
            {
                var edp = new IPEndPoint(IPAddressHelper.ResolveIPAddress(peer.Url), peer.Port);
                await Send(pkg, edp, TokenSource.Token);
            }
        }

        #endregion

        #region Check election

        private Task CheckElection(object source, ElapsedEventArgs e)
        {
            _logger.LogInformation("{Node}:{PeerId}:{TermId}, Check election {State}, nb votes {NbVotes}", _nodeId, _peerId, Info.TermId, State, _nbPositiveVote);
            if (State != PeerStates.CANDIDATE || _expirationCheckElectionDateTime == null) return Task.CompletedTask;
            var currentDateTime = DateTime.UtcNow;
            if (_nbPositiveVote >= _quorum)
            {
                _logger.LogInformation("{Node}:{PeerId}:{TermId}, Peer is now the leader !", _nodeId, _peerId, Info.TermId);
                StartBroadcastLeaderHeartbeat();
                SetLeader();
                return Task.CompletedTask;
            }

            if (_expirationCheckElectionDateTime.Value <= currentDateTime)
            {
                _logger.LogInformation("{Node}:{PeerId}:{TermId}, Finish to check the election", _nodeId, _peerId, Info.TermId);
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
            var pkg = ConsensusPackageRequestBuilder.LeaderHeartbeat(_nodeOptions.Url, _nodeOptions.Port, Info.TermId, Info.TermIndex);
            var nodes = await _clusterStore.GetAllNodes(TokenSource.Token);
            foreach (var peer in nodes)
            {
                try
                {
                    var ipEdp = new IPEndPoint(IPAddressHelper.ResolveIPAddress(peer.Url), peer.Port);
                    await Send(pkg, ipEdp, TokenSource.Token);
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    return;
                }
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
            if (State == PeerStates.FOLLOWER) return;
            State = PeerStates.FOLLOWER;
            Info.Reset();
            StartCheckLeaderHeartbeat(true);
            StopCheckElection();
            StopBroadcastLeaderHeartbeat();
            if (PeerIsFollower != null)
            {
                PeerIsFollower(this, new PeerHostEventArgs(Info.TermId, _nodeId, _peerId));
            }
        }

        private void SetCandidate()
        {
            if (State == PeerStates.CANDIDATE) return;
            State = PeerStates.CANDIDATE;
            StopCheckLeaderHeartbeat();
            StartCheckElection();
            StopBroadcastLeaderHeartbeat();
            if (PeerIsCandidate != null) PeerIsCandidate(this, new PeerHostEventArgs(Info.TermId, _nodeId, _peerId));
        }

        private void SetLeader()
        {
            if (State == PeerStates.LEADER) return;
            State = PeerStates.LEADER;
            StopCheckLeaderHeartbeat();
            StopCheckElection();
            StartBroadcastLeaderHeartbeat();
            if (PeerIsLeader != null) PeerIsLeader(this, new PeerHostEventArgs(Info.TermId, _nodeId, _peerId));
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
                    await HandlePackage(receiveResult);
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
                if (consensusPackage.Header.Command == ConsensusCommands.APPEND_ENTRY_REQUEST) result = await Handle(consensusPackage as AppendEntryRequest, cancellationToken);
                if (consensusPackage.Header.Command == ConsensusCommands.LEADER_HEARTBEAT_RESULT) await Handle(consensusPackage as LeaderHeartbeatResult, cancellationToken);
                if (result == null) return false;
                var ipEdp = new IPEndPoint(IPAddressHelper.ResolveIPAddress(consensusPackage.Header.SourceUrl), consensusPackage.Header.SourcePort);
                await Send(result, ipEdp, cancellationToken);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        private async Task<ConsensusPackage> Handle(LeaderHeartbeatRequest consensusPackage, CancellationToken cancellationToken)
        {
            if (consensusPackage.Header.TermId != Info.TermId) return null; ;
            _activeLeader = new LeaderNode { Port = consensusPackage.Header.SourcePort, Url = consensusPackage.Header.SourceUrl, LastLeaderHeartbeatReceivedDateTime = DateTime.UtcNow };
            for(var i = 0; i < _appendEntryRequestLst.Count(); i++)
            {
                var appendEntry = _appendEntryRequestLst.ElementAt(i);
                await Send(appendEntry, cancellationToken);
                _appendEntryRequestLst.Remove(appendEntry);
            }

            if (State == PeerStates.CANDIDATE) SetFollower();
            _logger.LogInformation("{Node}:{PeerId}:{TermId}:{State}, LearderHearbeatRequest {ConfirmedIndex}", _nodeId, _peerId, Info.TermId, State, Info.ConfirmedTermIndex);
            return ConsensusPackageResultBuilder.LeaderHeartbeat(_nodeOptions.Url, _nodeOptions.Port, Info.TermId, Info.ConfirmedTermIndex);
        }

        private Task<ConsensusPackage> Handle(VoteRequest voteRequest, CancellationToken cancellationToken)
        {
            var isGranted = true;
            if (Info.TermId != voteRequest.Header.TermId) isGranted = false;
            else
            {
                if (voteRequest.Header.TermIndex > Info.ConfirmedTermIndex)
                {
                    SetFollower();
                } 
                else if (
                    (State == PeerStates.LEADER || State == PeerStates.CANDIDATE) ||
                    (Info.ConfirmedTermIndex >= voteRequest.Header.TermIndex)
                )
                {
                    isGranted = false;
                }
            }

            return Task.FromResult(ConsensusPackageResultBuilder.Vote(_nodeOptions.Url, _nodeOptions.Port, Info.TermId, Info.TermIndex, isGranted));
        }

        private Task<ConsensusPackage> Handle(VoteResult voteResult, CancellationToken cancellationToken)
        {
            if (voteResult.VoteGranted) _nbPositiveVote++;
            return Task.FromResult((ConsensusPackage)null);
        }

        private async Task<ConsensusPackage> Handle(AppendEntryRequest appendEntry, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Node}:{PeerId}:{TermId}, Receive log {State}", _nodeId, _peerId, Info.TermId, State);
            if (State == PeerStates.LEADER || appendEntry.IsProxified)
            {
                await AppendEntry(appendEntry, false, cancellationToken);
                return null;
            }

            // Transfer to leader
            if(_activeLeader != null && _activeLeader.IsActive(_options.LeaderHeartbeatExpirationDurationMS))
            {
                await Send(appendEntry, cancellationToken);
                return null;
            }
            else _appendEntryRequestLst.Add(appendEntry);
            return null;
        }

        private async Task Handle(LeaderHeartbeatResult leaderHeartbeatResult, CancellationToken cancellationToken)
        {
            if (State != PeerStates.LEADER) return;
            if (Info.TermId == leaderHeartbeatResult.Header.TermId && Info.ConfirmedTermIndex > leaderHeartbeatResult.Header.TermIndex)
            {
                _logger.LogInformation("{Node}:{PeerId}:{TermId}, Receive heartbeat {ConfirmedTermIndex} > {TermIndex}", _nodeId, _peerId, Info.TermId, Info.ConfirmedTermIndex, leaderHeartbeatResult.Header.TermIndex);
                var index = leaderHeartbeatResult.Header.TermIndex + 1;
                var log = await _logStore.Get(index, cancellationToken);
                if (log == null) return;
                var pkg = ConsensusPackageRequestBuilder.AppendEntry(Info.TermId, index, log.Value, true);
                var edp = new IPEndPoint(IPAddressHelper.ResolveIPAddress(leaderHeartbeatResult.Header.SourceUrl), leaderHeartbeatResult.Header.SourcePort);
                await Send(pkg, edp, TokenSource.Token);
            }
        }

        private async Task Send(AppendEntryRequest appendEntry, CancellationToken cancellationToken)
        {
            var edp = new IPEndPoint(IPAddressHelper.ResolveIPAddress(_activeLeader.Url), _activeLeader.Port);
            await Send(appendEntry, edp, cancellationToken);
        }

        #endregion

        private UdpClient BuildUdpServer()
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
            await _udpClient.SendAsync(resultPayload, resultPayload.Count(), ipEdp).WithCancellation(cancellationToken);
        }

        private async Task AppendEntry(AppendEntryRequest appendEntry, bool forceAdd, CancellationToken cancellationToken)
        {
            if (appendEntry.IsProxified && appendEntry.Header.TermIndex <= Info.ConfirmedTermIndex) return;
            var logRecord = new LogRecord { Value = appendEntry.Value, Index = Info.ConfirmedTermIndex + 1, InsertionDateTime = DateTime.UtcNow };
            await AppendEntry(logRecord, forceAdd, cancellationToken);
        }
    }

    public class LeaderNode
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public DateTime? LastLeaderHeartbeatReceivedDateTime { get; set; }

        public bool IsActive(int durationMS)
        {
            var currentDateTime = DateTime.UtcNow;
            return LastLeaderHeartbeatReceivedDateTime != null && LastLeaderHeartbeatReceivedDateTime.Value.AddMilliseconds(durationMS) >= currentDateTime;
        }
    }
}
