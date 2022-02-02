using EventMesh.Runtime.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public interface IMessageHandler
    {
        EventMeshCommands Command { get; }
        Task<EventMeshPackage> Run(EventMeshPackage package, CancellationToken cancellationToken);
    }
}
