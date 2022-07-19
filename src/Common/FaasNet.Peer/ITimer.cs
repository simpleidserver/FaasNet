using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer
{
    public interface ITimer
    {
        Task Start(CancellationToken cancellationToken);
        void Stop();
    }
}
