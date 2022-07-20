using FaasNet.DHT.Kademlia.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core.Handlers
{
    public interface IRequestHandler
    {
        public KademliaCommandTypes Command { get; }
        Task<KademliaPackage> Handle(KademliaPackage request, CancellationToken cancellationToken);
    }
}
