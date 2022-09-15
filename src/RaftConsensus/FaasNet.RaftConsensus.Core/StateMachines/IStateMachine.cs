using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public interface IStateMachine
    {
        Task Apply(ICommand cmd, CancellationToken cancellationToken);
        Task Truncate(CancellationToken cancellationToken);
        Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken);
        Task Commit(CancellationToken cancellationToken);
        IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords);
        Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken);
    }
}
