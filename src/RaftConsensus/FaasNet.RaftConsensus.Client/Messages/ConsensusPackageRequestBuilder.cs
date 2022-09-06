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

        public static BaseConsensusPackage GetPeerState()
        {
            return new GetPeerStateRequest();
        }

        public static BaseConsensusPackage GetLogs(int index)
        {
            return new GetLogsRequest
            {
                StartIndex = index
            };
        }

        public static BaseConsensusPackage AppendEntry(string stateMachineId, byte[] payload)
        {
            return new AppendEntryRequest
            {
                StateMachineId = stateMachineId,
                Payload = payload
            };
        }

        public static BaseConsensusPackage InstallSnapshot(long term, string leaderId, long commitIndex, long snapshotTerm, long snapshotIndex, byte[] data, string stateMachineId)
        {
            return new InstallSnapshotRequest
            {
                Term = term,
                LeaderId = leaderId,
                CommitIndex = commitIndex,
                SnapshotTerm = snapshotTerm,
                SnapshotIndex = snapshotIndex,
                Data = data,
                StateMachineId = stateMachineId
            };
        }

        public static BaseConsensusPackage GetStateMachine(string stateMachineId)
        {
            return new GetStateMachineRequest
            {
                StateMachineId = stateMachineId
            };
        }

        public static BaseConsensusPackage ReadStateMachine(long offset)
        {
            return new ReadStateMachineRequest
            {
                Offset = offset
            };
        }

        public static BaseConsensusPackage GetAllStateMachines()
        {
            return new GetAllStateMachinesRequest();
        }
    }
}
