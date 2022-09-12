using FaasNet.Common.Helpers;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Client.StateMachines;
using FaasNet.RaftConsensus.Core.Infos;
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
                        if (otherPeer == null)
                        {
                            otherPeer = _peerInfo.AddOtherPeer(peer.Id);
                            await Heartbeat(edp, otherPeer);
                        }
                        else await PropagateLogEntries(edp, otherPeer);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            async Task Heartbeat(IPEndPoint edp, OtherPeerInfo otherPeer)
            {
                using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                {
                    var result = (await consensusClient.AppendEntries(_peerState.CurrentTerm, _peerOptions.Id, 0, 0, new List<LogEntry>(), _peerState.CommitIndex, _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                    otherPeer.MatchIndex = result.Item1.MatchIndex;
                }
            }

            async Task PropagateLogEntries(IPEndPoint edp, OtherPeerInfo otherPeer)
            {
                using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                {
                    var logs = new List<LogEntry>();
                    long previousIndex = 0, previousTerm = 0;
                    var previousLog = await _logStore.Get(otherPeer.MatchIndex, _cancellationTokenSource.Token);
                    if (previousLog != null)
                    {
                        previousIndex = previousLog.Index;
                        previousTerm = previousLog.Term;
                    }

                    if (otherPeer.NextIndex != null)
                    {
                        var log = await _logStore.Get(otherPeer.NextIndex.Value, _cancellationTokenSource.Token);
                        if (log != null) logs.Add(log);
                    }

                    var result = (await consensusClient.AppendEntries(_peerState.CurrentTerm, _peerOptions.Id, previousIndex, previousTerm, logs, _peerState.CommitIndex, _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                    otherPeer.MatchIndex = result.Item1.MatchIndex;
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
            var stateMachines = await GetStateMachines();
            foreach(var stateMachine in stateMachines)
            {
                var result = await InstallSnapshot(clusterPeers, stateMachine);
                TryCommit(result.Item2, result.Item1);
            }

            async Task<IEnumerable<(IStateMachine, Snapshot)>> GetStateMachines()
            {
                IEnumerable<(IStateMachine, Snapshot)> result = null;
                if (takeSnapshot)
                {
                    var logEntries = await _logStore.GetFromTo(_peerState.SnapshotCommitIndex, _peerState.CommitIndex, _cancellationTokenSource.Token);
                    result =_snapshotHelper.RestoreAllStateMachines(logEntries);
                }
                else result = _snapshotHelper.GetAllStateMachines();
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
                        if (otherPeer.GetSnapshotIndex(snapshot.StateMachineId) != snapshot.Index) data = snapshot.StateMachine;
                        var result = (await consensusClient.InstallSnapshot(_peerState.CurrentTerm, _peerOptions.Id, _peerState.SnapshotCommitIndex, snapshot.Term, snapshot.Index, data, snapshot.StateMachineId, _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                        otherPeer.UpdateSnapshotIndex(snapshot.StateMachineId, result.Item1.MatchIndex);
                        return result.Item1;
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
                _snapshotStore.Update(snapshot);
            }
        }
    }
}
