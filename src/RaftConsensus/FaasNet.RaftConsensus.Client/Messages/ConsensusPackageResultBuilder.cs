namespace FaasNet.RaftConsensus.Client.Messages
{
    public static class ConsensusPackageResultBuilder
    {
        public static BaseConsensusPackage Vote(long term, bool voteGranted)
        {
            return new VoteResult
            {
                Term = term,
                VoteGranted = voteGranted
            };
        }

        public static BaseConsensusPackage AppendEntries(long term, long matchIndex, bool success)
        {
            return new AppendEntriesResult
            {
                Term = term,
                Success = success,
                MatchIndex = matchIndex
            };
        }

        public static BaseConsensusPackage AppendEntry(long term, long matchIndex, bool success)
        {
            return new AppendEntriesResult
            {
                Term = term,
                Success = success,
                MatchIndex = matchIndex
            };
        }
    }
}
