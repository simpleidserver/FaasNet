using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Extensions;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Session
{
    public interface ISessionStateMachineStore : IStateMachineRecordStore<SessionRecord>
    {
        Task<GenericSearchResult<SessionRecord>> Find(string clientId, string vpn, FilterQuery filter, CancellationToken cancellationToken);
    }

    public class SessionStateMachineStore : ISessionStateMachineStore
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

        public Task<GenericSearchResult<SessionRecord>> Find(string clientId, string vpn, FilterQuery filter, CancellationToken cancellationToken)
        {
            var filteredSessions = _sessions.Where(s => s.ClientId == clientId && s.Vpn == vpn);
            IEnumerable<SessionRecord> filtered = filteredSessions.AsQueryable().InvokeOrderBy(filter.SortBy, filter.SortOrder).ToList();
            var totalRecords = filtered.Count();
            filtered = filtered.Skip(filter.NbRecords * filter.Page).Take(filter.NbRecords);
            var nbPages = (int)Math.Ceiling((decimal)totalRecords / filter.NbRecords);
            return Task.FromResult(new GenericSearchResult<SessionRecord>
            {
                Records = filtered,
                TotalPages = nbPages,
                TotalRecords = totalRecords
            });
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
