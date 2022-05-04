namespace FaasNet.RaftConsensus.Client.Messages
{
    public static class PackageResultBuilder
    {
        public static ConsensusPackage Vote(string termId, long termIndex, bool voteGranted)
        {
            return new VoteResult
            {
                Header = new Header(ConsensusCommands.VOTE_RESULT, termId, termIndex),
                VoteGranted = voteGranted
            };
        }

        public static ConsensusPackage Empty(string termId, long termIndex)
        {
            return new EmptyResult
            {
                Header = new Header(ConsensusCommands.EMPTY_RESULT, termId, termIndex)
            };
        }

        public static ConsensusPackage LeaderHeartbeat(string url, int port, string termId, long termIndex)
        {
            return new LeaderHeartbeatResult
            {
                Header = new Header(ConsensusCommands.LEADER_HEARTBEAT_RESULT, termId, termIndex),
                Port = port,
                Url = url
            };
        }
    }
}
