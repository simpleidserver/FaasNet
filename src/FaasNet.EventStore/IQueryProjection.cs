using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public interface IQueryProjection
    {
        Task Start(CancellationToken cancellationToken);
        void Stop();
    }
}
