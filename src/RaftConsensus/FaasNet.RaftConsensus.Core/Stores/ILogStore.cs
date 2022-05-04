using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface ILogStore
    {
        Task<IEnumerable<LogRecord>> GetAll(CancellationToken cancellationToken);
        Task<LogRecord> Get(long index, CancellationToken cancellationToken);
        void Add(LogRecord logRecord);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class InMemoryLogStore : ILogStore
    {
        private readonly ConcurrentBag<LogRecord> _logRecords;

        public InMemoryLogStore()
        {
            _logRecords = new ConcurrentBag<LogRecord>();
        }

        public void Add(LogRecord logRecord)
        {
            _logRecords.Add(logRecord);
        }

        public Task<LogRecord> Get(long index, CancellationToken cancellationToken)
        {
            return Task.FromResult(_logRecords.FirstOrDefault(lr => lr.Index == index));
        }

        public Task<IEnumerable<LogRecord>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<LogRecord> result = _logRecords.ToArray();
            return Task.FromResult(result);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
