using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface IClusterStore
    {
        Task<IEnumerable<Peer>> GetAllPeers(CancellationToken cancellationToken);
    }
}
