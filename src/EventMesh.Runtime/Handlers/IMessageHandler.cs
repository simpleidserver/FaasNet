using EventMesh.Runtime.Messages;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public interface IMessageHandler
    {
        EventMeshCommands Command { get; }
        Task<EventMeshPackage> Run(EventMeshPackage package, IPEndPoint sender, CancellationToken cancellationToken);
    }
}
