using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Infrastructure
{
    public interface IDistributedLock
    {
        Task<bool> TryAcquireLock(string id, CancellationToken token);
        Task ReleaseLock(string id, CancellationToken token);
    }
}
