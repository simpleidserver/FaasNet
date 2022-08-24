using FaasNet.Common.Helpers;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public partial class RaftConsensusTimer
    {
        private async Task StartCandidate()
        {
            if (_peerInfo.Status != PeerStatus.CANDIDATE) return;
            var allPeers = (await _clusterStore.GetAllNodes(_peerOptions.PartitionKey, _cancellationTokenSource.Token)).Where(p => p.Id != _peerOptions.Id);
            var voteResultLst = await Vote(allPeers);
            var quorum = (allPeers.Count() / 2) + 1;
            var nbReachablePeers = voteResultLst.Where(v => v.Item2).Count();
            var nbNegativeVote = voteResultLst.Where(p => p.Item2 && !p.Item1.VoteGranted).Count();
            if(nbNegativeVote > 0 || nbReachablePeers < quorum)
            {
                _peerInfo.MoveToFollower();
                return;
            }

            _peerInfo.MoveToLeader();
        }

        private async Task<IEnumerable<(VoteResult, bool)>> Vote(IEnumerable<ClusterPeer> peers)
        {
            _peerState.VotedFor = _peerOptions.Id;
            _peerState.IncreaseCurrentTerm();
            _logger.LogInformation($"Peer {_peerOptions.Id} has current term {_peerState.CurrentTerm}");
            var tasks = new List<Task<(VoteResult, bool)>>();
            foreach (var peer in peers) tasks.Add(SendRequest(peer));
            return await Task.WhenAll(tasks);
            
            async Task<(VoteResult, bool)> SendRequest(ClusterPeer peer)
            {
                try
                {
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(edp))
                    {
                        var voteResult = (await consensusClient.Vote(_peerOptions.Id, _peerState.CurrentTerm, _peerState.CommitIndex, _peerState.LastApplied, _raftOptions.RequestExpirationTimeMS, _cancellationTokenSource.Token)).First();
                        return (voteResult, true);
                    }
                }
                catch
                {
                    return (null, false);
                }
            }
        }
    }
}
