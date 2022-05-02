﻿namespace FaasNet.RaftConsensus.Client.Messages
{
    public class PackageRequestBuilder
    {
        public static ConsensusPackage LeaderHeartbeat(string termId, long termIndex)
        {
            return new LeaderHeartbeatRequest
            {
                Header = new Header(ConsensusCommands.LEADER_HEARTBEAT_REQUEST, termId, termIndex)
            };
        }

        public static ConsensusPackage Vote(string termId, long termIndex)
        {
            return new VoteRequest
            {
                Header = new Header(ConsensusCommands.VOTE_REQUEST, termId, termIndex)
            };
        }
    }
}
