using FaasNet.Common.Helpers;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.StateMachines;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public partial class RaftConsensusTimer
    {
        private System.Timers.Timer _leaderTimer;

        private void CreateLeaderTimer()
        {
            _leaderTimer = new System.Timers.Timer();
            _leaderTimer.Elapsed += async (o, e) =>
            {
                var allPeers = (await _clusterStore.GetAllNodes(_peerOptions.PartitionKey, _cancellationTokenSource.Token)).Where(p => p.Id != _peerOptions.Id);
                await AppendEntries(allPeers);
                TakeSnapshot(allPeers);
                StartLeader();
            };
            _leaderTimer.AutoReset = false;
        }

        private void StartLeader()
        {
            if (_peerInfo.Status != PeerStatus.LEADER) return;
            _leaderTimer.Interval = _raftOptions.LeaderHeartbeatTimerMS;
            _leaderTimer.Start();
        }

        private void StopLeaderTimer()
        {
            _leaderTimer.Stop();
        }

        private async Task AppendEntries(IEnumerable<ClusterPeer> peers)
        {
            var tasks = new List<Task>();
            foreach (var peer in peers) tasks.Add(AppendEntries(peer));
            await Task.WhenAll(tasks);
            TryCommit();

            async Task AppendEntries(ClusterPeer peer)
            {
                try
                {
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                    {
                        var otherPeer = _peerInfo.GetOtherPeer(peer.Id);
                        AppendEntriesResult result = null;
                        if (otherPeer == null)
                        {
                            result = (await consensusClient.Heartbeat(_peerState.CurrentTerm, _peerOptions.Id, _peerState.CommitIndex, _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                            otherPeer = _peerInfo.AddOtherPeer(peer.Id);
                        }
                        else
                        {
                            var previousTerm = _peerState.PreviousTerm;
                            var logs = new List<LogEntry>();
                            if (otherPeer.NextIndex != null)
                            {
                                var log = await _logStore.Get(otherPeer.NextIndex.Value, _cancellationTokenSource.Token);
                                if (log != null) logs.Add(log);
                            }

                            result = (await consensusClient.AppendEntries(_peerState.CurrentTerm, _peerOptions.Id, otherPeer.MatchIndex.Value, previousTerm, logs, _peerState.CommitIndex, _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                        }

                        otherPeer.MatchIndex = result.MatchIndex;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            void TryCommit()
            {
                var otherPeers = _peerInfo.OtherPeerInfos;
                var nextCommitIndex = _peerState.CommitIndex + 1;
                var nbMatchIndexes = otherPeers.Count(p => p.MatchIndex == nextCommitIndex);
                var quorum = (otherPeers.Count() / 2) + 1;
                if (quorum > nbMatchIndexes) return;
                _peerState.CommitIndex++;
            }
        }

        private async void TakeSnapshot(IEnumerable<ClusterPeer> clusterPeers)
        {
            var takeSnapshot = _peerState.CommitIndex - _peerState.SnapshotCommitIndex >= _raftOptions.SnapshotFrequency;
            var stateMachine = await GetStateMachine();
            var result = await InstallSnapshot(clusterPeers, stateMachine.Value);
            TryCommit(result.Item2, result.Item1);

            async Task<(IStateMachine, Snapshot)?> GetStateMachine()
            {
                (IStateMachine, Snapshot)? result = null;
                if (takeSnapshot)
                {
                    var logEntries = await _logStore.GetTo(_peerState.CommitIndex, _cancellationTokenSource.Token);
                    var stateMachine = _stateMachineStore.RestoreStateMachine(logEntries);
                    var snapshot = new Snapshot
                    {
                        Index = _peerState.CommitIndex,
                        StateMachine = stateMachine.Serialize(),
                        Term = logEntries.OrderByDescending(e => e.Term).First().Term
                    };
                    result = (stateMachine, snapshot);
                }
                else result = await _stateMachineStore.GetLatestStateMachine(_cancellationTokenSource.Token);
                return result;
            }

            async Task<(IEnumerable<InstallSnapshotResult>, Snapshot)> InstallSnapshot(IEnumerable<ClusterPeer> peers, (IStateMachine, Snapshot) stateMachine)
            {
                var tasks = new List<Task<InstallSnapshotResult>>();
                foreach (var peer in peers) tasks.Add(SendInstallSnapshot(stateMachine.Item2, peer));
                var result = await Task.WhenAll(tasks);
                return (result, stateMachine.Item2);
            }

            async Task<InstallSnapshotResult> SendInstallSnapshot(Snapshot snapshot, ClusterPeer peer)
            {
                try
                {
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                    {
                        byte[] data = null;
                        var otherPeer = _peerInfo.GetOtherPeer(peer.Id);
                        if (otherPeer.SnapshotIndex != snapshot.Index) data = snapshot.StateMachine;
                        var result = (await consensusClient.InstallSnapshot(_peerState.CurrentTerm, _peerOptions.Id, _peerState.SnapshotCommitIndex, snapshot.Term, snapshot.Index, data, _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                        otherPeer.MatchIndex = result.MatchIndex;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return null;
                }
            }

            async void TryCommit(Snapshot snapshot, IEnumerable<InstallSnapshotResult> installSnapshotResultLst)
            {
                var otherPeers = _peerInfo.OtherPeerInfos;
                var quorum = (otherPeers.Count() / 2) + 1;
                var nbMatchIndexes = installSnapshotResultLst.Where(s => s != null && s.Success).Count();
                if (quorum > nbMatchIndexes) return;
                _peerState.SnapshotCommitIndex = snapshot.Index;
                _peerState.SnapshotLastApplied = snapshot.Index;
                _stateMachineStore.Update(snapshot);
                await _logStore.RemoveTo(snapshot.Index, _cancellationTokenSource.Token);
            }
        }
    }
}
