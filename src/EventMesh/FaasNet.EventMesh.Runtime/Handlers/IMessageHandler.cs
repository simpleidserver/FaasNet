using FaasNet.EventMesh.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public interface IMessageHandler
    {
        Commands Command { get; }
        Task<Package> Run(Package package, CancellationToken cancellationToken);
    }
}
