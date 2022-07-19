using FaasNet.Common.Helpers;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public interface IRaftConsensusPartitionTimerStore
    {
        RaftConsensusPartitionTimer Add(PartitionElectionRecord partition, CancellationTokenSource cancellationTokenSource);
        RaftConsensusPartitionTimer Get(string partitionId);
        ICollection<RaftConsensusPartitionTimer> GetAll();
    }

    public class RaftConsensusPartitionTimerStore : IRaftConsensusPartitionTimerStore
    {
        private readonly ITransport _transport;
        private readonly ILeaderPeerInfoStore _leaderPeerInfoStore;
        private readonly IPartitionElectionStore _partitionElectionStore;
        private readonly IClusterStore _clusterStore;
        private readonly IPendingRequestStore _pendingRequestStore;
        private readonly ILogStore _logStore;
        private readonly ILogger<RaftConsensusPartitionTimerStore> _logger;
        private readonly RaftConsensusPeerOptions _raftConsensusPeerOptions;
        private readonly PeerOptions _peerOptions;
        private readonly ICollection<RaftConsensusPartitionTimer> _timers;


        public RaftConsensusPartitionTimerStore(ITransport transport, ILeaderPeerInfoStore leaderPeerInfoStore, IPartitionElectionStore partitionElectionStore, IClusterStore clusterStore, IPendingRequestStore pendingRequestStore, ILogStore logStore, ILogger<RaftConsensusPartitionTimerStore> logger, IOptions<RaftConsensusPeerOptions> raftConsensusPeerOptions, IOptions<PeerOptions> peerOptions)
        {
            _transport = transport;
            _leaderPeerInfoStore = leaderPeerInfoStore;
            _partitionElectionStore = partitionElectionStore;
            _clusterStore = clusterStore;
            _pendingRequestStore = pendingRequestStore;
            _logStore = logStore;
            _logger = logger;
            _raftConsensusPeerOptions = raftConsensusPeerOptions.Value;
            _peerOptions = peerOptions.Value;
            _timers = new List<RaftConsensusPartitionTimer>();
        }

        public RaftConsensusPartitionTimer Add(PartitionElectionRecord partition, CancellationTokenSource cancellationTokenSource)
        {
            var record = new RaftConsensusPartitionTimer(_leaderPeerInfoStore, _partitionElectionStore, _clusterStore, _transport, _pendingRequestStore, _logStore, _logger, partition.PartitionId, cancellationTokenSource, _raftConsensusPeerOptions, _peerOptions);
            _timers.Add(record);
            return record;
        }

        public RaftConsensusPartitionTimer Get(string partitionId)
        {
            return _timers.Single(t => t.PartitionId == partitionId);
        }

        public ICollection<RaftConsensusPartitionTimer> GetAll()
        {
            return _timers;
        }
    }

    public class RaftConsensusPartitionTimer
    {
        private readonly ILeaderPeerInfoStore _leaderPeerInfoStore;
        private readonly IPartitionElectionStore _partitionElectionStore;
        private readonly IClusterStore _clusterStore;
        private readonly ITransport _transport;
        private readonly IPendingRequestStore _pendingRequestStore;
        private readonly ILogStore _logStore;
        private readonly ILogger<RaftConsensusPartitionTimerStore> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly RaftConsensusPeerOptions _raftConsensusPeerOptions;
        private readonly PeerOptions _peerOptions;
        private readonly string _partitionId;
        private DateTime? _electionStartDateTime = null;
        private DateTime? _electionEndDateTime = null;
        private System.Timers.Timer _checkLeaderHeartbeatTimer;
        private System.Timers.Timer _electionCheckTimer;
        private System.Timers.Timer _leaderHeartbeatTimer;

        public RaftConsensusPartitionTimer(ILeaderPeerInfoStore leaderPeerInfoStore, IPartitionElectionStore partitionPeerInfoStore, IClusterStore clusterStore, ITransport transport, IPendingRequestStore pendingRequestStore, ILogStore logStore, ILogger<RaftConsensusPartitionTimerStore> logger, string partitionId, CancellationTokenSource cancellationTokenSource, RaftConsensusPeerOptions raftConsensusPeerOptions, PeerOptions peerOptions)
        {
            _leaderPeerInfoStore = leaderPeerInfoStore;
            _partitionElectionStore = partitionPeerInfoStore;
            _clusterStore = clusterStore;
            _transport = transport;
            _pendingRequestStore = pendingRequestStore;
            _logStore = logStore;
            _logger = logger;
            _partitionId = partitionId;
            _cancellationTokenSource = cancellationTokenSource;
            _raftConsensusPeerOptions = raftConsensusPeerOptions;
            _peerOptions = peerOptions;
        }

        public string PartitionId => _partitionId;

        public async Task Start()
        {
            var partition = await _partitionElectionStore.Get(_partitionId, _cancellationTokenSource.Token);
            _checkLeaderHeartbeatTimer = new System.Timers.Timer(_raftConsensusPeerOptions.CheckLeaderHeartbeatTimerMS);
            _checkLeaderHeartbeatTimer.Elapsed += async (o, e) => await CheckHeartbeat();
            _checkLeaderHeartbeatTimer.AutoReset = false;
            _electionCheckTimer = new System.Timers.Timer(_raftConsensusPeerOptions.CheckElectionTimerMS);
            _electionCheckTimer.Elapsed += async (o, e) => await CheckElection();
            _electionCheckTimer.AutoReset = false;
            _leaderHeartbeatTimer = new System.Timers.Timer(_raftConsensusPeerOptions.LeaderHeartbeatTimerMS);
            _leaderHeartbeatTimer.Elapsed += async (o, e) => await BroadcastLeaderHeartbeat();
            _leaderHeartbeatTimer.AutoReset = false;
            SetFollower(partition);
        }

        public void Stop()
        {
            _checkLeaderHeartbeatTimer.Stop();
            _electionCheckTimer.Stop();
            _leaderHeartbeatTimer.Stop();
        }

        private async Task CheckHeartbeat()
        {
            var partition = await _partitionElectionStore.Get(_partitionId, _cancellationTokenSource.Token);
            if (partition.PeerState != PeerStates.FOLLOWER) return;
            var leader = await _leaderPeerInfoStore.Get(_partitionId, _cancellationTokenSource.Token);
            if (leader != null && leader.IsActive(_raftConsensusPeerOptions.LeaderHeartbeatExpirationDurationMS))
            {
                if (partition.PeerState == PeerStates.CANDIDATE) partition.PeerState = PeerStates.FOLLOWER;
                StartCheckLeaderHeartbeat();
                return;
            }

            await StartElection(partition);
        }

        private async Task StartElection(PartitionElectionRecord partition)
        {
            if (_electionStartDateTime == null) _electionStartDateTime = DateTime.UtcNow.AddMilliseconds(_raftConsensusPeerOptions.ElectionIntervalMS.GetValue());
            if(_electionStartDateTime.Value > DateTime.UtcNow)
            {
                Thread.Sleep(50);
                await StartElection(partition);
                return;
            }

            var peers = await _clusterStore.GetAllNodes(_cancellationTokenSource.Token);
            peers = peers.Where(n => n.Port != _peerOptions.Port || n.Url != _peerOptions.Url);
            partition.Quorum = !peers.Any() ? 0 : (peers.Count() / 2) + 1;
            partition.NbPositiveVote = 0;
            SetCandidate(partition);
            var pkg = ConsensusPackageRequestBuilder.Vote(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
            var bufferCtx = new WriteBufferContext();
            pkg.SerializeEnvelope(bufferCtx);
            foreach (var peer in peers)
            {
                var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                await _transport.Send(bufferCtx.Buffer.ToArray(), edp, _cancellationTokenSource.Token);
            }
        }

        private async Task CheckElection()
        {
            var partition = await _partitionElectionStore.Get(_partitionId, _cancellationTokenSource.Token);
            if (partition.PeerState != PeerStates.CANDIDATE || _electionEndDateTime == null) return;
            var currentDateTime = DateTime.UtcNow;
            if (partition.NbPositiveVote >= partition.Quorum)
            {
                StartBroadcastLeaderHeartbeat();
                await SetLeader(partition, _cancellationTokenSource.Token);
                return;
            }

            if (_electionEndDateTime.Value <= currentDateTime)
            {
                SetFollower(partition);
                return;
            }

            StartCheckElection();
        }

        private async Task BroadcastLeaderHeartbeat()
        {
            var partition = await _partitionElectionStore.Get(_partitionId, _cancellationTokenSource.Token);
            if (partition.PeerState != PeerStates.LEADER) return;
            var pkg = ConsensusPackageRequestBuilder.LeaderHeartbeat(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
            var ctx = new WriteBufferContext();
            pkg.SerializeEnvelope(ctx);
            var nodes = await _clusterStore.GetAllNodes(_cancellationTokenSource.Token);
            var filteredNodes = nodes.Where(n => n.Url != _peerOptions.Url || n.Port != _peerOptions.Port);
            foreach (var peer in filteredNodes)
            {
                try
                {
                    var ipEdp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    await _transport.Send(ctx.Buffer.ToArray(), ipEdp, _cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    return;
                }
            }

            StartBroadcastLeaderHeartbeat();
        }

        public void SetFollower(PartitionElectionRecord partition)
        {
            Debug.WriteLine($"PeerId {_peerOptions.Port} becomes a follower");
            _logger.LogInformation($"PeerId {_peerOptions.Port} becomes a follower");
            _electionStartDateTime = null;
            partition.PeerState = PeerStates.FOLLOWER;
            partition.Reset();
            StartCheckLeaderHeartbeat();
            StopCheckElection();
            StopBroadcastLeaderHeartbeat();
        }

        private void SetCandidate(PartitionElectionRecord partition)
        {
            Debug.WriteLine($"PeerId {_peerOptions.Port} becomes a candidate");
            _logger.LogInformation($"PeerId {_peerOptions.Port} becomes a candidate");
            partition.PeerState = PeerStates.CANDIDATE;
            StopCheckLeaderHeartbeat();
            _electionEndDateTime = DateTime.UtcNow.AddMilliseconds(_raftConsensusPeerOptions.ElectionCheckDurationMS);
            StartCheckElection();
            StopBroadcastLeaderHeartbeat();
        }

        private async Task SetLeader(PartitionElectionRecord partition, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"PeerId {_peerOptions.Port} becomes a leader");
            _logger.LogInformation($"PeerId {_peerOptions.Port} becomes a leader");
            partition.PeerState = PeerStates.LEADER;
            StopCheckLeaderHeartbeat();
            StopCheckElection();
            StartBroadcastLeaderHeartbeat();
            var pendingRequestLst = _pendingRequestStore.GetAll();
            foreach(var pendingRequest in pendingRequestLst)
            {
                var logRecord = new LogRecord { ReplicationId = pendingRequest.Header.TermId, Value = pendingRequest.Value, Index = partition.ConfirmedTermIndex + 1, InsertionDateTime = DateTime.UtcNow };
                _logStore.Add(logRecord);
                partition.Upgrade();
            }

            await _partitionElectionStore.Update(partition, cancellationToken);
        }

        private void StartCheckLeaderHeartbeat()
        {
            _checkLeaderHeartbeatTimer?.Start();
        }

        private void StopCheckLeaderHeartbeat()
        {
            _checkLeaderHeartbeatTimer?.Stop();
        }

        private void StartCheckElection()
        {
            _electionCheckTimer?.Start();
        }

        private void StopCheckElection()
        {
            _electionCheckTimer?.Stop();
        }

        private void StartBroadcastLeaderHeartbeat()
        {
            _leaderHeartbeatTimer?.Start();
        }

        private void StopBroadcastLeaderHeartbeat()
        {
            _leaderHeartbeatTimer?.Stop();
        }
    }
}
