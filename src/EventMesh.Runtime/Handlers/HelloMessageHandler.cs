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
            if (!await _aclService.Check(helloRequest.UserAgent, PossibleActions.AUTHENTICATE, cancellationToken))
            {
                throw new RuntimeException(helloRequest.Header.Command, helloRequest.Header.Seq, Errors.NOT_AUTHORIZED);
            }

            TryCreateSession(helloRequest, sender);
            return PackageResponseBuilder.Hello(package.Header.Seq);
        }

        private void TryCreateSession(HelloRequest request, IPEndPoint sender)
        {
            bool isUpdated = true;
            var client = _clientSessionStore.Get(request.UserAgent.ClientId);
            if (client == null)
            {
                client = Client.Create(request.UserAgent.ClientId, request.UserAgent.Urn);
                isUpdated = false;
            }

            client.AddSession(sender,
                request.UserAgent.Environment, 
                request.UserAgent.Pid, 
                request.Header.Seq, 
                request.UserAgent.Purpose, 
                request.UserAgent.BufferCloudEvents, 
                request.UserAgent.IsServer);
            if (isUpdated)
            {
                _clientSessionStore.Update(client);
            }
            else
            {
                _clientSessionStore.Add(client);
            }
        }
    }
}
