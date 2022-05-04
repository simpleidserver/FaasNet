namespace FaasNet.RaftConsensus.Client.Messages
{
    public class PackageRequestBuilder
    {
        public static ConsensusPackage LeaderHeartbeat(string termId, long termIndex, string url, int port)
        {
            return new LeaderHeartbeatRequest
            {
                Header = new Header(ConsensusCommands.LEADER_HEARTBEAT_REQUEST, termId, termIndex),
                Url = url,
                Port = port
            };
        }

        public static ConsensusPackage Vote(string termId, long termIndex)
        {
            return new VoteRequest
            {
                Header = new Header(ConsensusCommands.VOTE_REQUEST, termId, termIndex)
            };
        }

        public static ConsensusPackage AppendEntry(string termId, long termIndex, string value, bool isProxified = false)
        {
            return new AppendEntryRequest
            {
                Header = new Header(ConsensusCommands.APPEND_ENTRY_REQUEST, termId, termIndex),
                Value = value,
                IsProxified = isProxified
            };
        }
    }
}
