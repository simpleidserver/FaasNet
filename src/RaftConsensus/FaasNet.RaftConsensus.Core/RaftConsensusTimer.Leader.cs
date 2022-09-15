﻿using FaasNet.Common.Helpers;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Infos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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
                Snapshot(allPeers);
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
            await TryCommit(_cancellationTokenSource.Token);

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

            async Task TryCommit(CancellationToken cancellationToken)
            {
                var otherPeers = _peerInfo.OtherPeerInfos;
                var nextCommitIndex = _peerState.CommitIndex + 1;
                var nbMatchIndexes = otherPeers.Count(p => p.MatchIndex == nextCommitIndex);
                var quorum = (otherPeers.Count() / 2) + 1;
                if (quorum > nbMatchIndexes) return;
                var commitIndex = _peerState.CommitIndex + 1;
                await _commitHelper.Commit(commitIndex, cancellationToken);
            }
        }

        private async void Snapshot(IEnumerable<ClusterPeer> clusterPeers)
        {
            var takeSnapshot = _peerState.CommitIndex - _peerState.SnapshotCommitIndex >= _raftOptions.SnapshotFrequency;
            if (takeSnapshot)
            {
                await TakeSnapshot();
                return;
            }

            var unknownPeers = clusterPeers.Where(cp =>
            {
                var otherPeer = _peerInfo.GetOtherPeer(cp.Id);
                return otherPeer == null || otherPeer.SnapshotCommitIndex != _peerState.SnapshotCommitIndex;
            }).ToList();
            var notSynchronizedPeers = clusterPeers.Where(cp => _peerInfo.GetOtherPeer(cp.Id).SnapshotIndex != _peerState.SnapshotCommitIndex).ToList();
            await Heartbeat(unknownPeers);
            await InstallSnapshot(notSynchronizedPeers, _peerState.SnapshotCommitIndex);

            async Task TakeSnapshot()
            {
                long newIndex = _peerState.SnapshotCommitIndex + 1;
                await _snapshotHelper.TakeSnapshot(newIndex);
                var nbSuccess = await InstallSnapshot(clusterPeers, newIndex);
                await TryCommit(newIndex, nbSuccess);
            }

            async Task<int> InstallSnapshot(IEnumerable<ClusterPeer> clusterPeers, long newIndex)
            {
                var peersInError = new ConcurrentBag<string>();
                var lst = new ConcurrentBag<InstallSnapshotResult>();
                foreach(var cp in _snapshotHelper.ReadSnapshot(newIndex))
                {
                    if (cp.Item1 == null) return 0;
                    foreach (var clusterPeer in clusterPeers)
                    {
                        try
                        {
                            var edp = new IPEndPoint(DnsHelper.ResolveIPV4(clusterPeer.Url), clusterPeer.Port);
                            using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                            {
                                var otherPeer = _peerInfo.GetOtherPeer(clusterPeer.Id);
                                if (otherPeer == null) otherPeer = _peerInfo.AddOtherPeer(clusterPeer.Id);
                                var result = (await consensusClient.InstallSnapshot(_peerState.CurrentTerm, _peerOptions.Id, _peerState.SnapshotCommitIndex,
                                    _peerState.SnapshotTerm, _peerState.SnapshotLastApplied, cp.Item2, cp.Item3, cp.Item1,
                                    _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                                if (result.Item1.Success) otherPeer.UpdateSnapshotIndex(result.Item1.MatchIndex);
                                else if (!peersInError.Contains(clusterPeer.Id)) peersInError.Add(clusterPeer.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                            if (!peersInError.Contains(clusterPeer.Id)) peersInError.Add(clusterPeer.Id);
                        }
                    }
                }

                return clusterPeers.Count() - peersInError.Count();
            }

            async Task TryCommit(long newIndex, int nbSuccess)
            {
                var otherPeers = _peerInfo.OtherPeerInfos;
                var quorum = (otherPeers.Count() / 2) + 1;
                if (quorum > nbSuccess) return;
                _peerState.SnapshotCommitIndex = newIndex;
                await _logStore.RemoveTo(newIndex, _cancellationTokenSource.Token);
            }

            async Task Heartbeat(IEnumerable<ClusterPeer> clusterPeers)
            {
                await Parallel.ForEachAsync(clusterPeers, new ParallelOptions
                {
                    MaxDegreeOfParallelism = _raftOptions.MaxNbThreads
                }, async (clusterPeer, t) =>
                {
                    var otherPeer = _peerInfo.GetOtherPeer(clusterPeer.Id);
                    if (otherPeer == null) otherPeer = _peerInfo.AddOtherPeer(clusterPeer.Id);
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(clusterPeer.Url), clusterPeer.Port);
                    try
                    {
                        using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                        {
                            var result = (await consensusClient.InstallSnapshot(_peerState.CurrentTerm, _peerOptions.Id, _peerState.SnapshotCommitIndex,
                                _peerState.SnapshotTerm, _peerState.SnapshotLastApplied, 0, 0, null,
                                _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                            if (result.Item1.Success)
                            {
                                otherPeer.UpdateSnapshotIndex(result.Item1.MatchIndex);
                                otherPeer.UpdateSnapshotCommitIndex(result.Item1.CommitIndex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                });
            }
        }
    }
}
