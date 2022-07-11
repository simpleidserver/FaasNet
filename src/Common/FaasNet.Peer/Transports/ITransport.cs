using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Transports
{
    public interface ITransport
    {
        void Start(CancellationToken cancellationToken = default(CancellationToken));
        void Stop();
        Task<MessageResult> ReceiveMessage();
    }
}
