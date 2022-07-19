namespace FaasNet.RaftConsensus.Client.Messages
{
    public static class ConsensusPackageResultBuilder
    {
        public static BaseConsensusPackage Vote(string url, int port, string termId, long termIndex, bool voteGranted)
        {
            return new VoteResult
            {
                Header = new ConsensusHeader(ConsensusCommands.VOTE_RESULT, termId, termIndex, url, port),
                VoteGranted = voteGranted
            };
        }

        public static BaseConsensusPackage LeaderHeartbeat(string url, int port, string termId, long termIndex)
        {
            return new LeaderHeartbeatResult
            {
                Header = new ConsensusHeader(ConsensusCommands.LEADER_HEARTBEAT_RESULT, termId, termIndex, url, port)
            };
        }

        public static BaseConsensusPackage Empty(string url, int port, string termId, long termIndex)
        {
            return new LeaderHeartbeatResult
            {
                Header = new ConsensusHeader(ConsensusCommands.EMPTY_RESULT, termId, termIndex, url, port)
            };
        }

        public static BaseConsensusPackage OkGet(string url, int port, string termId, long termIndex, string value)
        {
            return new GetEntryResult
            {
                Header = new ConsensusHeader(ConsensusCommands.GET_RESULT, termId, termIndex, url, port),
                Value = value,
                IsNotFound = false
            };
        }

        public static BaseConsensusPackage NotFoundGet(string url, int port, string termId, long termIndex)
        {
            return new GetEntryResult
            {
                Header = new ConsensusHeader(ConsensusCommands.GET_RESULT, termId, termIndex, url, port),
                IsNotFound = true
            };
        }
    }
}
