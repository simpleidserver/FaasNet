using System.Threading;

namespace FaasNet.Peer
{
    public interface ITimer
    {
        void Start(CancellationToken cancellationToken);
        void Stop();
    }
}
