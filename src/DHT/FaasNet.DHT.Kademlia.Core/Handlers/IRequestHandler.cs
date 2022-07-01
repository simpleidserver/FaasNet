using FaasNet.DHT.Kademlia.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core.Handlers
{
    public interface IRequestHandler
    {
        public Commands Command { get; }
        Task<BasePackage> Handle(BasePackage request, CancellationToken cancellationToken);
    }
}
