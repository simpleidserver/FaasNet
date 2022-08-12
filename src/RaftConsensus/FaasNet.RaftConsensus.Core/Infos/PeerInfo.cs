using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Core.Infos
{
    public class PeerInfo
    {
        public PeerInfo()
        {
            OtherPeerInfos = new List<OtherPeerInfo>();
        }

        /// <summary>
        /// Status of the Peer.
        /// </summary>
        public PeerStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VotedFor { get; set; }
        /// <summary>
        /// Last time heartbeat has been received from a leader.
        /// </summary>
        public DateTime? LeaderHeartbeatReceptionDateTime { get; set; }
        /// <summary>
        /// Information about the other Peers.
        /// </summary>
        public ICollection<OtherPeerInfo> OtherPeerInfos { get; set; }

        public event EventHandler FollowerStateStarted;
        public event EventHandler CandidateStateStarted;
        public event EventHandler LeaderStateStarted;

        public bool IsLeaderActive(int expirationSlidingTimeMS) => LeaderHeartbeatReceptionDateTime != null && DateTime.UtcNow < LeaderHeartbeatReceptionDateTime.Value.AddMilliseconds(expirationSlidingTimeMS);

        public void MoveToFollower()
        {
            Status = PeerStatus.FOLLOWER;
            if(FollowerStateStarted != null) FollowerStateStarted(this, null);
        }

        public void MoveToCandidate()
        {
            if (Status != PeerStatus.FOLLOWER) return;
            Status = PeerStatus.CANDIDATE;
            if (CandidateStateStarted != null) CandidateStateStarted(this, null);
        }

        public void MoveToLeader()
        {
            if (Status != PeerStatus.CANDIDATE) return;
            Status = PeerStatus.LEADER;
            if (LeaderStateStarted != null) LeaderStateStarted(this, null);
        }

        public OtherPeerInfo GetOtherPeer(string id)
        {
            return OtherPeerInfos.SingleOrDefault(p => p.Id == id);
        }
    }

    public class OtherPeerInfo
    {
        /// <summary>
        /// Unique Peer identifier.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Index of the next log entry to send to that server.
        /// </summary>
        public long? NextIndex { get; set; }
        /// <summary>
        /// Index of highest log entry known to be replicated on server.
        /// </summary>
        public long? MatchIndex { get; set; }
    }
}
