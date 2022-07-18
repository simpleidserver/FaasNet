using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Transports
{
    public interface ITransport
    {
        void Start(CancellationToken cancellationToken = default(CancellationToken));
        void Stop();
        Task Send(byte[] payload, IPEndPoint edp, CancellationToken cancellationToken);
        Task<MessageResult> ReceiveMessage();
    }
}
