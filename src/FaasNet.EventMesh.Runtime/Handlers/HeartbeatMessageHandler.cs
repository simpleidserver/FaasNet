using FaasNet.EventMesh.Runtime.Messages;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class HeartbeatMessageHandler : IMessageHandler
    {
        public Commands Command => Commands.HEARTBEAT_REQUEST;

        public Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            return Task.FromResult(PackageResponseBuilder.HeartBeat(package.Header.Seq));
        }
    }
}
