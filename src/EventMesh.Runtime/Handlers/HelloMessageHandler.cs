using EventMesh.Runtime.Messages;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class HelloMessageHandler : IMessageHandler
    {
        public EventMeshCommands Command => EventMeshCommands.HEARTBEAT_REQUEST;

        public Task<EventMeshPackage> Run(EventMeshPackage package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
