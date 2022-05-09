namespace FaasNet.RaftConsensus.Client.Messages.Consensus
{
    public class ConsensusPackageRequestBuilder
    {
        public static ConsensusPackage LeaderHeartbeat(string url, int port, string termId, long termIndex)
        {
            return new LeaderHeartbeatRequest
            {
                Header = new ConsensusHeader(ConsensusCommands.LEADER_HEARTBEAT_REQUEST, termId, termIndex, url, port)
            };
        }

        public static ConsensusPackage Vote(string url, int port, string termId, long termIndex)
        {
            return new VoteRequest
            {
                Header = new ConsensusHeader(ConsensusCommands.VOTE_REQUEST, termId, termIndex, url, port)
            };
        }

        public static ConsensusPackage AppendEntry(string termId, long termIndex, string value, bool isProxified = false)
        {
            return new AppendEntryRequest
            {
                Header = new ConsensusHeader(ConsensusCommands.APPEND_ENTRY_REQUEST, termId, termIndex, string.Empty, 0),
                Value = value,
                IsProxified = isProxified
            };
        }
    }
}
