using FaasNet.DHT.Kademlia.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core.Handlers
{
    public class PingRequestHandler : IRequestHandler
    {
        public Commands Command => Commands.PING_REQUEST;

        public Task<BasePackage> Handle(BasePackage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PackageResponseBuilder.Pong(request.Nonce));
        }
    }
}
