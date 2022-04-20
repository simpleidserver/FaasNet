using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime
{
    public interface IRuntimeHost
    {
        Task Run(CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
    }
}
