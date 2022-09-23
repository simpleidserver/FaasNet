using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Extensions;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Client
{
    public interface IClientStateMachineStore : IStateMachineRecordStore<ClientRecord>
    {
        Task<ClientRecord> Get(string id, string vpn, CancellationToken cancellationToken);
        Task<IEnumerable<ClientRecord>> GetAll(CancellationToken cancellationToken);
        Task<GenericSearchResult<ClientRecord>> Find(FilterQuery filter, CancellationToken cancellationToken);
    }

    public class ClientStateMachineStore : IClientStateMachineStore
    {
        private ConcurrentBag<ClientRecord> _clients;

        public ClientStateMachineStore()
        {
            _clients = new ConcurrentBag<ClientRecord>();
        }

        public void Add(ClientRecord record)
        {
            _clients.Add(record);
        }

        public Task BulkUpload(IEnumerable<ClientRecord> records, CancellationToken cancellationToken)
        {
            _clients = new ConcurrentBag<ClientRecord>(records);
            return Task.CompletedTask;
        }

        public Task<ClientRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.Id == key));
        }

        public Task<ClientRecord> Get(string id, string vpn, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.Id == id && c.Vpn == vpn));
        }

        public IEnumerable<(IEnumerable<ClientRecord>, int)> GetAll(int nbRecords)
        {
            int nbPages = (int)Math.Ceiling((double)_clients.Count / nbRecords);
            for (var currentPage = 0; currentPage < nbPages; currentPage++)
                yield return (_clients.Skip(currentPage * nbRecords).Take(nbRecords), currentPage);
        }

        public Task<IEnumerable<ClientRecord>> GetAll(CancellationToken cancellationToken)
        {
            return Task.FromResult((IEnumerable<ClientRecord>)_clients.OrderByDescending(c => c.CreateDateTime));
        }

        public Task<GenericSearchResult<ClientRecord>> Find(FilterQuery filter, CancellationToken cancellationToken)
        {
            IEnumerable<ClientRecord> filtered = _clients.AsQueryable().InvokeOrderBy(filter.SortBy, filter.SortOrder).ToList();
            var totalRecords = filtered.Count();
            filtered = filtered.Skip(filter.NbRecords * filter.Page).Take(filter.NbRecords);
            var nbPages = (int)Math.Ceiling((decimal)totalRecords / filter.NbRecords);
            return Task.FromResult(new GenericSearchResult<ClientRecord>
            {
                Records = filtered,
                TotalPages = nbPages,
                TotalRecords = totalRecords
            });
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            _clients = new ConcurrentBag<ClientRecord>();
            return Task.CompletedTask;
        }

        public void Update(ClientRecord record)
        {
        }
    }
}
