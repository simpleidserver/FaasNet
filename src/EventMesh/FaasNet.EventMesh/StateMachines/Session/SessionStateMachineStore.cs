using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Session
{
    public class SessionStateMachineStore : IStateMachineRecordStore<SessionRecord>
    {
        private ConcurrentBag<SessionRecord> _sessions = new ConcurrentBag<SessionRecord>();

        public void Add(SessionRecord record)
        {
            _sessions.Add(record);
        }

        public Task BulkUpload(IEnumerable<SessionRecord> records, CancellationToken cancellationToken)
        {
            _sessions = new ConcurrentBag<SessionRecord>(records);
            return Task.CompletedTask;
        }

        public Task<SessionRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_sessions.FirstOrDefault(s => s.Id == key));
        }

        public IEnumerable<(IEnumerable<SessionRecord>, int)> GetAll(int nbRecords)
        {
            int nbPages = (int)Math.Ceiling((double)_sessions.Count / nbRecords);
            for (var currentPage = 0; currentPage < nbPages; currentPage++)
                yield return (_sessions.Skip(currentPage * nbRecords).Take(nbRecords), currentPage);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            _sessions = new ConcurrentBag<SessionRecord>();
            return Task.CompletedTask;
        }

        public void Update(SessionRecord record)
        {
        }
    }
}
