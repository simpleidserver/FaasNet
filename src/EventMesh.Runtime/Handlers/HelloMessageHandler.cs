using EventMesh.Runtime.Acl;
using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class HelloMessageHandler : IMessageHandler
    {
        private readonly IClientStore _clientSessionStore;
        private readonly IACLService _aclService;

        public HelloMessageHandler(
            IClientStore clientSessionStore,
            IACLService aclService)
        {
            _clientSessionStore = clientSessionStore;
            _aclService = aclService;
        }

        public Commands Command => Commands.HELLO_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var helloRequest = package as HelloRequest;
            var client = _clientSessionStore.GetByActiveSession(sender);
            if (client == null)
            {
                await TryCreateSession(helloRequest, sender, cancellationToken);
            }

            return PackageResponseBuilder.Hello(package.Header.Seq);
        }

        private async Task TryCreateSession(HelloRequest request, IPEndPoint sender, CancellationToken cancellationToken)
        {
            if (!await _aclService.Check(request.UserAgent, PossibleActions.AUTHENTICATE, cancellationToken))
            {
                throw new RuntimeException(request.Header.Command, request.Header.Seq, Errors.NOT_AUTHORIZED);
            }

            var client = _clientSessionStore.Get(request.UserAgent.ClientId);
            if (client == null)
            {
                client = Client.Create(request.UserAgent.ClientId);
            }

            client.AddSession(sender, request.UserAgent.Environment, request.UserAgent.Pid, request.Header.Seq, request.UserAgent.Purpose, request.UserAgent.BufferCloudEvents);
            _clientSessionStore.Update(client);
            return;
        }
    }
}
