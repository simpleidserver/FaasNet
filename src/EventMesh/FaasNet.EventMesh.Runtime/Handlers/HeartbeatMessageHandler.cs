using FaasNet.EventMesh.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class HeartbeatMessageHandler : IMessageHandler
    {
        public Commands Command => Commands.HEARTBEAT_REQUEST;

        public Task<EventMeshPackageResult> Run(Package package, CancellationToken cancellationToken)
        {
            var result = PackageResponseBuilder.HeartBeat(package.Header.Seq);
            return Task.FromResult(EventMeshPackageResult.SendResult(result));
        }
    }
}
