using FaasNet.Common.Helpers;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
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
            _leaderTimer.Elapsed += (o, e) => AppendEntries();
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

        private async void AppendEntries()
        {
            var allPeers = (await _clusterStore.GetAllNodes(_cancellationTokenSource.Token)).Where(p => p.Id != _peerOptions.Id);
            await AppendEntries(allPeers);
            StartLeader();
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
                    using (var consensusClient = new UDPRaftConsensusClient(edp))
                    {
                        var otherPeer = _peerInfo.GetOtherPeer(peer.Id);
                        AppendEntriesResult result = null;
                        if (otherPeer == null)
                        {
                            result = await consensusClient.Heartbeat(_peerState.CurrentTerm, _peerOptions.Id, _peerState.CommitIndex, _cancellationTokenSource.Token);
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

                            result = await consensusClient.AppendEntries(_peerState.CurrentTerm, _peerOptions.Id, otherPeer.MatchIndex.Value, previousTerm, logs, _peerState.CommitIndex, _cancellationTokenSource.Token);
                        }

                        otherPeer.MatchIndex = result.MatchIndex;
                    }
                }
                catch(Exception ex)
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
    }
}
