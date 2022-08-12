namespace FaasNet.RaftConsensus.Core
{
    public enum PeerStatus
    {
        /// <summary>
        /// A node issues no requests but passively wits for requests to come in from the leader or candidates.
        /// </summary>
        FOLLOWER = 0,
        /// <summary>
        /// Used for leader election. When a follower detects the loss of the leader, they transition to the candidate state and start sending out RequestVote requests to all other nodes.
        /// </summary>
        CANDIDATE = 1,
        /// <summary>
        /// Responsible for interacting with clients and replicating the log to the followers.
        /// </summary>
        LEADER = 2
    }
}
