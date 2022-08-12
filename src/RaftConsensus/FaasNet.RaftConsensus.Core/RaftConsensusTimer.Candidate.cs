using FaasNet.Common.Helpers;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client.Messages;
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
            var allPeers = (await _clusterStore.GetAllNodes(_cancellationTokenSource.Token)).Where(p => p.Id != _peerOptions.Id);
            var voteResultLst = await Vote(allPeers);
            var quorum = (allPeers.Count() / 2) + 1;
            var nbReachablePeers = voteResultLst.Where(v => v.Item2).Count();
            var nbNegativeVote = voteResultLst.Where(p => p.Item2 && !p.Item1.VoteGranted).Count();
            if(nbNegativeVote > 0 || nbReachablePeers < quorum)
            {
                _peerInfo.MoveToFollower();
                return;
            }
            
            StartLeader();
        }

        private async Task<IEnumerable<(VoteResult, bool)>> Vote(IEnumerable<ClusterPeer> peers)
        {
            var tasks = new List<Task<(VoteResult, bool)>>();
            foreach (var peer in peers) tasks.Add(SendRequest(peer));
            return await Task.WhenAll(tasks);
            
            async Task<(VoteResult, bool)> SendRequest(ClusterPeer peer)
            {
                try
                {
                    var writeBufferCtx = new WriteBufferContext();
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(peer.Url), peer.Port);
                    var pkg = ConsensusPackageRequestBuilder.Vote(string.Empty, _peerState.CurrentTerm, _peerState.CommitIndex, _peerState.LastApplied);
                    pkg.SerializeEnvelope(writeBufferCtx);
                    await _transport.Send(writeBufferCtx.Buffer.ToArray(), edp, _cancellationTokenSource.Token);
                    var receivedPayload = await _transport.Receive(_cancellationTokenSource.Token);
                    var readBufferCtx = new ReadBufferContext(receivedPayload);
                    return (BaseConsensusPackage.Deserialize(readBufferCtx) as VoteResult, true);
                }
                catch
                {
                    return (null, false);
                }
            }
        }
    }
}
