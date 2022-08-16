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
        Task<long> GetPreviousTerm(long term, CancellationToken cancellationToken);
        Task<LogEntry> Get(long index, CancellationToken cancellationToken);
        Task<LogEntry> Get(long term, long index, CancellationToken cancellationToken);
        Task<IEnumerable<LogEntry>> GetFrom(long index, CancellationToken cancellationToken);
        Task RemoveFrom(long startIndex, CancellationToken cancellation);
        Task UpdateRange(IEnumerable<LogEntry> entries, CancellationToken cancellationToken);
        Task Append(LogEntry entry, CancellationToken cancellationToken);
    }

    public class InMemoryLogStore : ILogStore
    {
        private ConcurrentBag<LogEntry> _entries;

        public InMemoryLogStore()
        {
            _entries = new ConcurrentBag<LogEntry>();
        }

        public Task<long> GetPreviousTerm(long term, CancellationToken cancellationToken)
        {
            var entries = _entries.Select(e => e.Term).Distinct().OrderBy(t => t).ToList();
            var index = entries.IndexOf(term);
            if (index == -1 || index == 0) return Task.FromResult(term);
            return Task.FromResult(entries.ElementAt((int)term - 1));
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
            _entries = new ConcurrentBag<LogEntry>(_entries.Skip((int)startIndex).Take(_entries.Count()));
            return Task.CompletedTask;
        }

        public Task UpdateRange(IEnumerable<LogEntry> entries, CancellationToken cancellationToken)
        {
            foreach (var entry in entries) _entries.Add(entry);
            return Task.CompletedTask;
        }

        public Task Append(LogEntry entry, CancellationToken cancellationToken)
        {
            _entries.Add(entry);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<LogEntry>> GetFrom(long index, CancellationToken cancellationToken)
        {
            var result = _entries.Where(e => e.Index >= index);
            return Task.FromResult(result);
        }
    }
}
