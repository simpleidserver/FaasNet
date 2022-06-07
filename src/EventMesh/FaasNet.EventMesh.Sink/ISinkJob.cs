using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Sink
{
    public interface ISinkJob
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop();
    }
}
