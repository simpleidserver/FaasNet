using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class HelloMessageHandler : IMessageHandler
    {
        private readonly IVpnStore _vpnStore;
        private readonly IClientStore _clientStore;

        public HelloMessageHandler(IVpnStore vpnStore,
            IClientStore clientStore)
        {
            _vpnStore = vpnStore;
            _clientStore = clientStore;
        }

        public Commands Command => Commands.HELLO_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var helloRequest = package as HelloRequest;
            var vpn = await _vpnStore.Get(helloRequest.UserAgent.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new RuntimeException(helloRequest.Header.Command, helloRequest.Header.Seq, Errors.UNKNOWN_VPN);
            }

            var client = await _clientStore.GetByClientId(vpn.Name, helloRequest.UserAgent.ClientId, cancellationToken);
            if (client == null)
            {
                throw new RuntimeException(helloRequest.Header.Command, helloRequest.Header.Seq, Errors.INVALID_CLIENT);
            }

            if (!client.Purposes.Any(p => p == helloRequest.UserAgent.Purpose.Code))
            {
                throw new RuntimeException(helloRequest.Header.Command, helloRequest.Header.Seq, Errors.NOT_AUTHORIZED);
            }

            var sessionId = client.AddSession(sender,
                helloRequest.UserAgent.Environment,
                helloRequest.UserAgent.Pid,
                helloRequest.UserAgent.Purpose,
                helloRequest.UserAgent.BufferCloudEvents,
                helloRequest.UserAgent.IsServer,
                vpn.Name);
            _vpnStore.Update(vpn);
            await _vpnStore.SaveChanges(cancellationToken);
            return PackageResponseBuilder.Hello(package.Header.Seq, sessionId);
        }
    }
}
