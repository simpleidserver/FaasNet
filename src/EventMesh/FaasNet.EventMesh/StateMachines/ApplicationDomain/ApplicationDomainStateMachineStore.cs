using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Extensions;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.ApplicationDomain
{
    public interface IApplicationDomainStateMachineStore : IStateMachineRecordStore<ApplicationDomainRecord>
    {
        Task<ApplicationDomainRecord> Get(string key, string vpn, CancellationToken cancellationToken);
        Task<GenericSearchResult<ApplicationDomainRecord>> Find(FilterQuery filter, CancellationToken cancellationToken);
    }

    public class ApplicationDomainStateMachineStore : IApplicationDomainStateMachineStore
    {
        private ConcurrentBag<ApplicationDomainRecord> _records = new ConcurrentBag<ApplicationDomainRecord>();

        public ApplicationDomainStateMachineStore()
        {

        }

        public void Add(ApplicationDomainRecord record)
        {
            _records.Add(record);
        }

        public Task BulkUpload(IEnumerable<ApplicationDomainRecord> records, CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<ApplicationDomainRecord>(records);
            return Task.CompletedTask;
        }

        public Task<GenericSearchResult<ApplicationDomainRecord>> Find(FilterQuery filter, CancellationToken cancellationToken)
        {
            IEnumerable<ApplicationDomainRecord> filtered = _records.AsQueryable().InvokeOrderBy(filter.SortBy, filter.SortOrder).ToList();
            var totalRecords = filtered.Count();
            filtered = filtered.Skip(filter.NbRecords * filter.Page).Take(filter.NbRecords);
            var nbPages = (int)Math.Ceiling((decimal)totalRecords / filter.NbRecords);
            return Task.FromResult(new GenericSearchResult<ApplicationDomainRecord>
            {
                Records = filtered,
                TotalPages = nbPages,
                TotalRecords = totalRecords
            });
        }

        public Task<ApplicationDomainRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.FirstOrDefault(r => r.Name == key));
        }

        public Task<ApplicationDomainRecord> Get(string key, string vpn, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.FirstOrDefault(r => r.Name == key && r.Vpn == vpn));
        }

        public IEnumerable<(IEnumerable<ApplicationDomainRecord>, int)> GetAll(int nbRecords)
        {
            int nbPages = (int)Math.Ceiling((double)_records.Count / nbRecords);
            for (var currentPage = 0; currentPage < nbPages; currentPage++)
                yield return (_records.Skip(currentPage * nbRecords).Take(nbRecords), currentPage);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<ApplicationDomainRecord>();
            return Task.CompletedTask;
        }

        public void Update(ApplicationDomainRecord record)
        {
        }
    }
}
