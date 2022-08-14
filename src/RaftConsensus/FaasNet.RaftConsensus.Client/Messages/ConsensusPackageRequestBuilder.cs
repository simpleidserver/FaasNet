using System.Collections.Generic;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusPackageRequestBuilder
    {
        public static BaseConsensusPackage Vote(string candidateId, long term, long lastLogIndex, long lastLogTerm)
        {
            return new VoteRequest
            {
                CandidateId = candidateId,
                LastLogIndex = lastLogIndex,
                LastLogTerm = lastLogTerm,
                Term = term
            };
        }

        public static BaseConsensusPackage Heartbeat(long term, string leaderId, long leaderCommit)
        {
            return new AppendEntriesRequest
            {
                Term = term,
                LeaderId = leaderId,
                LeaderCommit = leaderCommit
            };
        }

        public static BaseConsensusPackage AppendEntries(long term, string leaderId, long prevLogIndex, long prevLogTerm, IEnumerable<LogEntry> entries, long leaderCommit)
        {
            return new AppendEntriesRequest
            {
                Term = term,
                LeaderId = leaderId,
                PrevLogIndex = prevLogIndex,
                PreLogTerm = prevLogTerm,
                Entries = entries,
                LeaderCommit = leaderCommit
            };
        }

        public static BaseConsensusPackage AppendEntry(byte[] payload)
        {
            return new AppendEntryRequest
            {
                Payload = payload
            };
        }
    }
}
