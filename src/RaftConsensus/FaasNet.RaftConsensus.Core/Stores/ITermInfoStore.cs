using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface ITermInfoStore
    {
        Task ResetFollower(CancellationToken cancellationToken);
        Task<IEnumerable<TermInfo>> GetAll(CancellationToken cancellationToken);
        Task<TermInfo> Get(string id, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
