namespace FaasNet.RaftConsensus.Client.Messages
{
    public static class PackageResultBuilder
    {
        public static ConsensusPackage Vote(string url, int port, string termId, long termIndex, bool voteGranted)
        {
            return new VoteResult
            {
                Header = new Header(ConsensusCommands.VOTE_RESULT, termId, termIndex, url, port),
                VoteGranted = voteGranted
            };
        }

        public static ConsensusPackage LeaderHeartbeat(string url, int port, string termId, long termIndex)
        {
            return new LeaderHeartbeatResult
            {
                Header = new Header(ConsensusCommands.LEADER_HEARTBEAT_RESULT, termId, termIndex, url, port)
            };
        }
    }
}
