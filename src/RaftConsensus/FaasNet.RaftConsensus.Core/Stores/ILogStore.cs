using FaasNet.RaftConsensus.Client;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface ILogStore
    {
        Task<LogEntry> Get(long index, CancellationToken cancellationToken);
        Task<LogEntry> Get(long term, long index, CancellationToken cancellationToken);
        Task RemoveFrom(long startIndex, CancellationToken cancellation);
        Task UpdateRange(IEnumerable<LogEntry> entries, CancellationToken cancellationToken);
    }

    public class InMemoryLogStore : ILogStore
    {
        private readonly ConcurrentBag<LogEntry> _entries;

        public InMemoryLogStore()
        {
            _entries = new ConcurrentBag<LogEntry>();
        }

        public Task<LogEntry> Get(long index, CancellationToken cancellationToken)
        {
            return Task.FromResult(_entries.SingleOrDefault(e => e.Index == index));
        }

        public Task<LogEntry> Get(long term, long index, CancellationToken cancellationToken)
        {
            return Task.FromResult(_entries.SingleOrDefault(e => e.Term == term && e.Index == index));
        }

        public Task RemoveFrom(long startIndex, CancellationToken cancellation)
        {
            return Task.CompletedTask;
        }

        public Task UpdateRange(IEnumerable<LogEntry> entries, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
