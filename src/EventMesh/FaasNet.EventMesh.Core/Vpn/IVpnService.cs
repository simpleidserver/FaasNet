using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn
{
    public interface IVpnService
    {
        Task<VpnResult> Get(string name, CancellationToken cancellationToken);
    }
}
