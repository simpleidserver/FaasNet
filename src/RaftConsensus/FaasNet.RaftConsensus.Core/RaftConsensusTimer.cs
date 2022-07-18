using FaasNet.Common.Helpers;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Stores;
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
    public class RaftConsensusTimer : ITimer
    {
        private readonly ITransport _transport;
        private readonly ILeaderPeerInfoStore _leaderPeerInfoStore;
        private readonly IPartitionElectionStore _partitionElectionStore;
        private readonly IClusterStore _clusterStore;
        private readonly RaftConsensusPeerOptions _raftConsensusPeerOptions;
        private readonly PeerOptions _peerOptions;
        private CancellationTokenSource _cancellationTokenSource;
        private ICollection<RaftConsensusPartitionTimer> _timers;

        public RaftConsensusTimer(ITransport transport, ILeaderPeerInfoStore leaderPeerInfoStore, IPartitionElectionStore partitionElectionStore, IClusterStore clusterStore, IOptions<RaftConsensusPeerOptions> raftConsensusPeerOptions, IOptions<PeerOptions> peerOptions)
        {
            _transport = transport;
            _leaderPeerInfoStore = leaderPeerInfoStore;
            _partitionElectionStore = partitionElectionStore;
            _clusterStore = clusterStore;
            _raftConsensusPeerOptions = raftConsensusPeerOptions.Value;
            _peerOptions = peerOptions.Value;
        }

        public async void Start(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _timers = new List<RaftConsensusPartitionTimer>();
            var partitions = await _partitionElectionStore.GetAll(cancellationToken);
            foreach (var partition in partitions)
            {
                _timers.Add(new RaftConsensusPartitionTimer(_leaderPeerInfoStore, _partitionElectionStore, _clusterStore, _transport, partition.PartitionId, _cancellationTokenSource, _raftConsensusPeerOptions, _peerOptions));
            }

            foreach(var timer in _timers)
            {
                timer.Start();
            }
        }

        public void Stop()
        {
            foreach(var timer in _timers)
            {
                timer.Stop();
            }
        }

        private class RaftConsensusPartitionTimer
        {
            private readonly ILeaderPeerInfoStore _leaderPeerInfoStore;
            private readonly IPartitionElectionStore _partitionElectionStore;
            private readonly IClusterStore _clusterStore;
            private readonly ITransport _transport;
            private readonly CancellationTokenSource _cancellationTokenSource;
            private readonly RaftConsensusPeerOptions _raftConsensusPeerOptions;
            private readonly PeerOptions _peerOptions;
            private readonly string _partitionId;
            private DateTime? _expirationCheckElectionDateTime = null;
            private System.Timers.Timer _checkLeaderHeartbeatTimer;
            private System.Timers.Timer _electionCheckTimer;
            private System.Timers.Timer _leaderHeartbeatTimer;

            public RaftConsensusPartitionTimer(ILeaderPeerInfoStore leaderPeerInfoStore, IPartitionElectionStore partitionPeerInfoStore, IClusterStore clusterStore, ITransport transport, string partitionId, CancellationTokenSource cancellationTokenSource, RaftConsensusPeerOptions raftConsensusPeerOptions, PeerOptions peerOptions)
            {
                _leaderPeerInfoStore = leaderPeerInfoStore;
                _partitionElectionStore = partitionPeerInfoStore;
                _clusterStore = clusterStore;
                _transport = transport;
                _partitionId = partitionId;
                _cancellationTokenSource = cancellationTokenSource;
                _raftConsensusPeerOptions = raftConsensusPeerOptions;
                _peerOptions = peerOptions;
            }

            public void Start()
            {
                _checkLeaderHeartbeatTimer = new System.Timers.Timer(_raftConsensusPeerOptions.CheckLeaderHeartbeatTimerMS);
                _checkLeaderHeartbeatTimer.Elapsed += async (o, e) => await CheckHeartbeat();
                _checkLeaderHeartbeatTimer.AutoReset = false;
                _electionCheckTimer = new System.Timers.Timer(_raftConsensusPeerOptions.CheckElectionTimerMS);
                _electionCheckTimer.Elapsed += async (o, e) => await CheckElection();
                _electionCheckTimer.AutoReset = false;
                _leaderHeartbeatTimer = new System.Timers.Timer(_raftConsensusPeerOptions.LeaderHeartbeatTimerMS);
                _leaderHeartbeatTimer.Elapsed += async (o, e) => await BroadcastLeaderHeartbeat();
                _leaderHeartbeatTimer.AutoReset = false;
                StartCheckLeaderHeartbeat(true);
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
                if (DateTime.UtcNow < _expirationCheckElectionDateTime)
                {
                    StartCheckLeaderHeartbeat(false);
                    return;
                }

                var leader = await _leaderPeerInfoStore.Get(_partitionId, _cancellationTokenSource.Token);
                if (leader != null && leader.IsActive(_raftConsensusPeerOptions.LeaderHeartbeatExpirationDurationMS))
                {
                    if (partition.PeerState == PeerStates.CANDIDATE) partition.PeerState = PeerStates.FOLLOWER;
                    StartCheckLeaderHeartbeat(false);
                    return;
                }

                await StartElection(partition);
            }

            private async Task StartElection(PartitionElectionRecord partition)
            {
                var currentDateTime = DateTime.UtcNow;
                if (_expirationCheckElectionDateTime != null && _expirationCheckElectionDateTime.Value > currentDateTime) return;
                // Start to vote.
                var peers = await _clusterStore.GetAllNodes(_cancellationTokenSource.Token);
                peers = peers.Where(n => n.Port != _peerOptions.Port || n.Url != _peerOptions.Url);
                partition.Quorum = !peers.Any() ? 0 : (peers.Count() / 2) + 1;
                partition.NbPositiveVote = 0;
                _expirationCheckElectionDateTime = DateTime.UtcNow.AddMilliseconds(_raftConsensusPeerOptions.ElectionCheckDurationMS);
                SetCandidate(partition);
                partition.Increment();
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
                if (partition.PeerState != PeerStates.CANDIDATE || _expirationCheckElectionDateTime == null) return;
                var currentDateTime = DateTime.UtcNow;
                if (partition.NbPositiveVote >= partition.Quorum)
                {
                    StartBroadcastLeaderHeartbeat();
                    SetLeader(partition);
                    return;
                }

                if (_expirationCheckElectionDateTime.Value <= currentDateTime)
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

            private void SetFollower(PartitionElectionRecord partition)
            {
                Debug.WriteLine($"PeerId {_peerOptions.Port} becomes a follower");
                partition.PeerState = PeerStates.FOLLOWER;
                partition.Reset();
                StartCheckLeaderHeartbeat(true);
                StopCheckElection();
                StopBroadcastLeaderHeartbeat();
            }

            private void SetCandidate(PartitionElectionRecord partition)
            {
                Debug.WriteLine($"PeerId {_peerOptions.Port} becomes a candidate");
                partition.PeerState = PeerStates.CANDIDATE;
                StopCheckLeaderHeartbeat();
                StartCheckElection();
                StopBroadcastLeaderHeartbeat();
            }

            private void SetLeader(PartitionElectionRecord partition)
            {
                Debug.WriteLine($"PeerId {_peerOptions.Port} becomes a leader");
                partition.PeerState = PeerStates.LEADER;
                StopCheckLeaderHeartbeat();
                StopCheckElection();
                StartBroadcastLeaderHeartbeat();
            }

            private void StartCheckLeaderHeartbeat(bool computeExpirationTime)
            {
                if (computeExpirationTime) _expirationCheckElectionDateTime = DateTime.UtcNow.AddMilliseconds(_raftConsensusPeerOptions.CheckLeaderHeartbeatIntervalMS.GetValue());
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
}
