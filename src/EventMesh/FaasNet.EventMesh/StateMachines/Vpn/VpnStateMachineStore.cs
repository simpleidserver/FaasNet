using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Extensions;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Vpn
{
    public interface IVpnStateMachineStore : IStateMachineRecordStore<VpnRecord>
    {
        Task<IEnumerable<VpnRecord>> GetAll(CancellationToken cancellationToken);
        Task<GenericSearchResult<VpnRecord>> Find(FilterQuery filter, CancellationToken cancellationToken);
    }

    public class VpnStateMachineStore : IVpnStateMachineStore
    {
        private ConcurrentBag<VpnRecord> _vpns;

        public VpnStateMachineStore()
        {
            _vpns = new ConcurrentBag<VpnRecord>();
        }

        public void Add(VpnRecord record)
        {
            _vpns.Add(record);
        }

        public Task BulkUpload(IEnumerable<VpnRecord> records, CancellationToken cancellationToken)
        {
            _vpns = new ConcurrentBag<VpnRecord>(records);
            return Task.CompletedTask;
        }

        public Task<VpnRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_vpns.FirstOrDefault(v => v.Name == key));
        }

        public IEnumerable<(IEnumerable<VpnRecord>, int)> GetAll(int nbRecords)
        {
            int nbPages = (int)Math.Ceiling((double)_vpns.Count / nbRecords);
            for (var currentPage = 0; currentPage < nbPages; currentPage++)
                yield return (_vpns.Skip(currentPage * nbRecords).Take(nbRecords), currentPage);
        }

        public Task<IEnumerable<VpnRecord>> GetAll(CancellationToken cancellationToken)
        {
            return Task.FromResult((IEnumerable<VpnRecord>)_vpns.OrderByDescending(v => v.UpdateDateTime));
        }

        public Task<GenericSearchResult<VpnRecord>> Find(FilterQuery filter, CancellationToken cancellationToken)
        {
            IEnumerable<VpnRecord> filtered = _vpns.AsQueryable().InvokeOrderBy(filter.SortBy, filter.SortOrder).ToList();
            var totalRecords = filtered.Count();
            filtered = filtered.Skip(filter.NbRecords * filter.Page).Take(filter.NbRecords);
            var nbPages = (int)Math.Ceiling((decimal)totalRecords / filter.NbRecords);
            return Task.FromResult(new GenericSearchResult<VpnRecord>
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
            _vpns = new ConcurrentBag<VpnRecord>();
            return Task.CompletedTask;
        }

        public void Update(VpnRecord record)
        {
        }
    }
}
