using FaasNet.DHT.Kademlia.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core.Handlers
{
    public class PingRequestHandler : IRequestHandler
    {
        public KademliaCommandTypes Command => KademliaCommandTypes.PING_REQUEST;

        public Task<KademliaPackage> Handle(KademliaPackage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PackageResponseBuilder.Pong(request.Nonce));
        }
    }
}
