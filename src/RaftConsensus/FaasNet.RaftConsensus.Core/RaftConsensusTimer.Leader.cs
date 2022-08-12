using FaasNet.Common.Helpers;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Generic;
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
            _leaderTimer.Interval = _raftOptions.LeaderHeartbeatTimerMS;
            _leaderTimer.Start();
        }

        private void StopLeaderTimer()
        {
            _leaderTimer.Stop();
        }

        private async void AppendEntries()
        {
            var peers = await _clusterStore.GetAllNodes(_cancellationTokenSource.Token);
            await AppendEntries(peers);
        }

        private async Task AppendEntries(IEnumerable<ClusterPeer> peers)
        {
            var tasks = new List<Task>();
            foreach (var peer in peers) tasks.Add(AppendEntries(peer));
            await Task.WhenAll(tasks);

            async Task AppendEntries(ClusterPeer peer)
            {
                var result = await SendRequest(peer);
                if (!result.Item2) return;
                var otherPeer = _peerInfo.GetOtherPeer(peer.Id);
                otherPeer.MatchIndex = result.Item1.MatchIndex;
            }

            async Task<(AppendEntriesResult, bool)> SendRequest(ClusterPeer peer)
            {
                try
                {
                    var writeBufferCtx = new WriteBufferContext();
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    var pkg = await BuildAppendEntries(peer);
                    pkg.SerializeEnvelope(writeBufferCtx);
                    await _transport.Send(writeBufferCtx.Buffer.ToArray(), edp, _cancellationTokenSource.Token);
                    var receivedPayload = await _transport.Receive(_cancellationTokenSource.Token);
                    var readBufferCtx = new ReadBufferContext(receivedPayload);
                    return (BaseConsensusPackage.Deserialize(readBufferCtx) as AppendEntriesResult, true);
                }
                catch
                {
                    return (null, false);
                }
            }

            async Task<BaseConsensusPackage> BuildAppendEntries(ClusterPeer peer)
            {
                var otherPeer = _peerInfo.GetOtherPeer(peer.Id);
                if(otherPeer == null) return ConsensusPackageRequestBuilder.Heartbeat(_peerState.CurrentTerm, _peerOptions.Id, _peerState.CommitIndex);
                var log = await _logStore.Get(otherPeer.NextIndex.Value, _cancellationTokenSource.Token);
                return ConsensusPackageRequestBuilder.AppendEntries(log.Term, _peerOptions.Id, otherPeer.MatchIndex.Value, 0, log == null ? new List<LogEntry>() : new List<LogEntry> { log }, _peerState.CommitIndex);
            }
        }
    }
}
