using System.Collections.Generic;

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

        public static BaseConsensusPackage AppendEntry(long term, long matchIndex, long lastIndex, bool success)
        {
            return new AppendEntryResult
            {
                Term = term,
                Success = success,
                MatchIndex = matchIndex,
                LastIndex = lastIndex
            };
        }

        public static BaseConsensusPackage GetPeerState(long term, string votedFor, long commitIndex, long lastApplied, PeerStatus status, long snapshotLastApplied, long snapshotCommitIndex)
        {
            return new GetPeerStateResult
            {
                Term = term,
                VotedFor = votedFor,
                CommitIndex = commitIndex,
                LastApplied = lastApplied,
                Status = status,
                SnapshotLastApplied = snapshotLastApplied,
                SnapshotCommitIndex = snapshotCommitIndex
            };
        }

        public static BaseConsensusPackage GetLogs(IEnumerable<LogEntry> entries)
        {
            return new GetLogsResult
            {
                Entries = entries
            };
        }

        public static BaseConsensusPackage InstallSnapshot(bool success)
        {
            return new InstallSnapshotResult
            {
                Success = success
            };
        }

        public static BaseConsensusPackage Query(IQueryResult result)
        {
            return new QueryResult
            {
                IsFound = true,
                Result = result
            };
        }

        public static BaseConsensusPackage Query()
        {
            return new QueryResult
            {
                IsFound = false
            };
        }
    }
}
