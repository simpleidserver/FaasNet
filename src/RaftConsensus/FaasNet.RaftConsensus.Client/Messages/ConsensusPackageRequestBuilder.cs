namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusPackageRequestBuilder
    {
        public static BaseConsensusPackage LeaderHeartbeat(string url, int port, string termId, long termIndex)
        {
            return new LeaderHeartbeatRequest
            {
                Header = new ConsensusHeader(ConsensusCommands.LEADER_HEARTBEAT_REQUEST, termId, termIndex, url, port)
            };
        }

        public static BaseConsensusPackage Vote(string url, int port, string termId, long termIndex)
        {
            return new VoteRequest
            {
                Header = new ConsensusHeader(ConsensusCommands.VOTE_REQUEST, termId, termIndex, url, port)
            };
        }

        public static BaseConsensusPackage AppendEntry(string termId, long termIndex, string value, bool isProxified = false)
        {
            return new AppendEntryRequest
            {
                Header = new ConsensusHeader(ConsensusCommands.APPEND_ENTRY_REQUEST, termId, termIndex, string.Empty, 0),
                Value = value,
                IsProxified = isProxified
            };
        }

        public static BaseConsensusPackage GetEntry(string termId)
        {
            return new GetEntryRequest
            {
                Header = new ConsensusHeader(ConsensusCommands.GET_REQUEST, termId, 0, string.Empty, 0)
            };
        }
    }
}
