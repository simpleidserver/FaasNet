using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Seed
{
    public interface ISeedJob
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop();
    }
}
