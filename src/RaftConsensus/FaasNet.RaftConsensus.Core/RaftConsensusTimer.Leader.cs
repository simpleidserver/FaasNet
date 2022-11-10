using FaasNet.Common.Helpers;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.Infos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public partial class RaftConsensusTimer
    {
        private SemaphoreSlim _sem = new SemaphoreSlim(1);
        private System.Timers.Timer _leaderTimer;

        private void CreateLeaderTimer()
        {
            _leaderTimer = new System.Timers.Timer();
            _leaderTimer.Elapsed += async (o, e) =>
            {
                await Commit();
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

        private async Task Commit()
        {
            if (_peerInfo.Status != PeerStatus.LEADER) return;
            await _sem.WaitAsync();
            var allPeers = (await _clusterStore.GetAllNodes(_peerOptions.PartitionKey, _cancellationTokenSource.Token)).Where(p => p.Id != _peerOptions.Id);
            await AppendEntries(allPeers);
            Snapshot();
            _sem.Release();
        }

        private async Task AppendEntries(IEnumerable<ClusterPeer> peers)
        {
            var sw = new Stopwatch();
            sw.Start();
            var tasks = new List<Task<(ClusterPeer, List<LogEntry>)>>();
            var result = new List<(ClusterPeer, List<LogEntry>)>();
            foreach (var peer in peers) tasks.Add(AppendEntries(peer));
            var lst = await Task.WhenAll(tasks);
            await TryCommit(lst, _cancellationTokenSource.Token);
            sw.Stop();
            var filtered = lst.Where(r => r.Item2.Any());
            if(filtered.Any())
                _logger.LogInformation("entries {entries} have been appended into the peers in {appendEntriesExceutionTimeMS}MS", filtered.Select(r => $"Peer: {r.Item1.Url}:{r.Item1.Port}, Entries: {r.Item2.Count()}"), sw.ElapsedMilliseconds);
            
            async Task<(ClusterPeer, List<LogEntry>)> AppendEntries(ClusterPeer peer)
            {
                try
                {
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    var otherPeer = _peerInfo.GetOtherPeer(peer.Id);
                    if (otherPeer == null) otherPeer = _peerInfo.AddOtherPeer(peer.Id);
                    await Heartbeat(edp, otherPeer);
                    if (otherPeer.MatchIndex < _peerState.SnapshotLastApplied)
                    {
                        var key = $"InstallSnapshot{peer.Id}";
                        if(!_taskPool.IsTaskExists(key))
                        {
#pragma warning disable 4014
                            _taskPool.Enqueue(() => InstallSnapshot(edp, otherPeer), key);
#pragma warning restore 4014
                        }
                    }
                    else return (peer, await PropagateLogEntries(edp, otherPeer));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }

                return (peer, new List<LogEntry>());
            }

            async Task Heartbeat(IPEndPoint edp, OtherPeerInfo otherPeer)
            {
                using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                {
                    var result = (await consensusClient.AppendEntries(_peerState.CurrentTerm, _peerOptions.Id, 0, 0, new List<LogEntry>(), _peerState.CommitIndex, _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                    otherPeer.MatchIndex = result.Item1.MatchIndex;
                }
            }

            async Task InstallSnapshot(IPEndPoint edp, OtherPeerInfo otherPeer)
            {
                foreach (var snapshotChunk in _snapshotHelper.ReadSnapshot(_peerState.SnapshotCommitIndex))
                {
                    using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                    {
                        var result = (await consensusClient.InstallSnapshot(_peerOptions.Id, _peerState.SnapshotTerm,
                            _peerState.SnapshotLastApplied, snapshotChunk.CurrentChunck, snapshotChunk.NbChuncks, snapshotChunk.Content,
                            _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                        if (!result.Item1.Success) return;
                    }
                }
            }

            async Task<List<LogEntry>> PropagateLogEntries(IPEndPoint edp, OtherPeerInfo otherPeer)
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
                    return logs;
                }
            }

            async Task TryCommit(IEnumerable<(ClusterPeer, List<LogEntry>)> peers, CancellationToken cancellationToken)
            {
                var otherPeers = _peerInfo.OtherPeerInfos;
                var nextCommitIndex = _peerState.CommitIndex + 1;
                var nbMatchIndexes = otherPeers.Count(p => p.MatchIndex == nextCommitIndex);
                var quorum = (otherPeers.Count() / 2) + 1;
                if (quorum > nbMatchIndexes) return;
                var commitIndex = _peerState.CommitIndex + 1;
                await _commitHelper.Commit(commitIndex, cancellationToken);
                await Parallel.ForEachAsync(peers.Where(p => p.Item2.Any()), new ParallelOptions
                {
                    MaxDegreeOfParallelism = _raftOptions.MaxNbThreads
                }, async (s, t) =>
                {
                    var otherPeer = _peerInfo.GetOtherPeer(s.Item1.Id);
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(s.Item1.Url), s.Item1.Port);
                    using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                    {
                        var result = (await consensusClient.AppendEntries(_peerState.CurrentTerm, _peerOptions.Id, 0, 0, new List<LogEntry>(), _peerState.CommitIndex, _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                        otherPeer.MatchIndex = result.Item1.MatchIndex;
                    }
                });
            }
        }

        private async void Snapshot()
        {
            var takeSnapshot = _peerState.CommitIndex - _peerState.SnapshotLastApplied >= _raftOptions.SnapshotFrequency;
            if (takeSnapshot)
            {
                await TakeSnapshot();
                return;
            }

            async Task TakeSnapshot()
            {
                Debug.WriteLine("Take a snapshot");
                long newIndex = _peerState.SnapshotCommitIndex + 1;
                await _snapshotHelper.TakeSnapshot(newIndex);
                await _logStore.RemoveTo(_peerState.CommitIndex, _cancellationTokenSource.Token);
                _logger.LogInformation("Snapshot {snapshotCommitIndex} has been taken, last index is {snapshotLastIndex}", newIndex, _peerState.SnapshotLastApplied);
            }
        }
    }
}
