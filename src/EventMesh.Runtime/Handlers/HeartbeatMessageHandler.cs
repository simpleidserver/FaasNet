using EventMesh.Runtime.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class HeartbeatMessageHandler : IMessageHandler
    {
        public EventMeshCommands Command => EventMeshCommands.HEARTBEAT_REQUEST;

        public Task<EventMeshPackage> Run(EventMeshPackage package, CancellationToken cancellationToken)
        {
            return Task.FromResult(EventMeshMessageResponseBuilder.HeartBeat(package.Header.Seq));
        }
    }
}
