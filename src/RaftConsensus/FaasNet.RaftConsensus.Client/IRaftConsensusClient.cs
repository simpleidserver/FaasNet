using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Client
{
    public interface IRaftConsensusClient
    {
        Task<IEnumerable<VoteResult>> Vote(string candidateId, long currentTerm, long commitIndex, long lastApplied, CancellationToken cancellationToken);
        Task<IEnumerable<AppendEntriesResult>> Heartbeat(long currentTerm, string candidateId, long commitIndex, CancellationToken cancellationToken);
        Task<IEnumerable<AppendEntriesResult>> AppendEntries(long term, string leaderId, long prevLogIndex, long prevLogTerm, IEnumerable<LogEntry> entries, long leaderCommit, CancellationToken cancellationToken);
        Task<IEnumerable<AppendEntryResult>> AppendEntry(byte[] payload, CancellationToken cancellationToken);
        Task<IEnumerable<GetPeerStateResult>> GetPeerState(CancellationToken cancellationToken);
        Task<IEnumerable<GetLogsResult>> GetLogs(int index, CancellationToken cancellationToken);
    }
}
