using FaasNet.RaftConsensus.Client;
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
            var previous = Status;
            Status = PeerStatus.FOLLOWER;
            if(FollowerStateStarted != null) FollowerStateStarted(this, new PeerInfoStateChanged(previous, Status));
        }

        public void MoveToCandidate()
        {
            if (Status != PeerStatus.FOLLOWER) return;
            var previous = Status;
            Status = PeerStatus.CANDIDATE;
            if (CandidateStateStarted != null) CandidateStateStarted(this, new PeerInfoStateChanged(previous, Status));
        }

        public void MoveToLeader()
        {
            if (Status != PeerStatus.CANDIDATE) return;
            var previous = Status;
            Status = PeerStatus.LEADER;
            if (LeaderStateStarted != null) LeaderStateStarted(this, new PeerInfoStateChanged(previous, Status));
        }

        public OtherPeerInfo GetOtherPeer(string id) => OtherPeerInfos.SingleOrDefault(p => p.Id == id);
        public OtherPeerInfo AddOtherPeer(string id)
        {
            var record = new OtherPeerInfo { Id = id };
            OtherPeerInfos.Add(record);
            return record;
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
        public long? NextIndex => MatchIndex + 1;
        /// <summary>
        /// Index of highest log entry known to be replicated on server.
        /// </summary>
        public long MatchIndex { get; set; } = 0;
        /// <summary>
        /// Index of the snapshot.
        /// </summary>
        public long SnapshotIndex { get; set; } = 0;
    }

    public class PeerInfoStateChanged : EventArgs
    {
        public PeerInfoStateChanged(PeerStatus previousStatus, PeerStatus newStatus)
        {
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
        }

        public PeerStatus PreviousStatus { get; private set; }
        public PeerStatus NewStatus { get; private set; }
        public bool IsDifferent => PreviousStatus != NewStatus;
    }
}
