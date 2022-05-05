﻿namespace FaasNet.RaftConsensus.Client.Messages
{
    public class PackageRequestBuilder
    {
        public static ConsensusPackage LeaderHeartbeat(string url, int port, string termId, long termIndex)
        {
            return new LeaderHeartbeatRequest
            {
                Header = new Header(ConsensusCommands.LEADER_HEARTBEAT_REQUEST, termId, termIndex, url, port)
            };
        }

        public static ConsensusPackage Vote(string url, int port, string termId, long termIndex)
        {
            return new VoteRequest
            {
                Header = new Header(ConsensusCommands.VOTE_REQUEST, termId, termIndex, url, port)
            };
        }

        public static ConsensusPackage AppendEntry(string termId, long termIndex, string value, bool isProxified = false)
        {
            return new AppendEntryRequest
            {
                Header = new Header(ConsensusCommands.APPEND_ENTRY_REQUEST, termId, termIndex, string.Empty, 0),
                Value = value,
                IsProxified = isProxified
            };
        }
    }
}
