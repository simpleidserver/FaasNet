using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public interface IStateMachineRecordStore<TRecord> where TRecord : IRecord
    {
        void Add(TRecord record);
        void Update(TRecord record);
        Task<int> SaveChanges(CancellationToken cancellationToken);
        IEnumerable<(IEnumerable<TRecord>, int)> GetAll(int nbRecords);
        Task<TRecord> Get(string key, CancellationToken cancellationToken);
        Task Truncate(CancellationToken cancellationToken);
        Task BulkUpload(IEnumerable<TRecord> records, CancellationToken cancellationToken);
    }
}
