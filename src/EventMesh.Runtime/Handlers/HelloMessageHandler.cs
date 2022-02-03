using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Stores;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class HelloMessageHandler : IMessageHandler
    {
        private readonly IClientSessionStore _clientSessionStore;

        public HelloMessageHandler(IClientSessionStore clientSessionStore)
        {
            _clientSessionStore = clientSessionStore;
        }

        public Commands Command => Commands.HELLO_REQUEST;

        public Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var helloRequest = package as HelloRequest;
            var clientSession = _clientSessionStore.Get(sender);
            if (clientSession == null)
            {
                _clientSessionStore.Add(new Session(sender, helloRequest.UserAgent));
            }

            return Task.FromResult(PackageResponseBuilder.Hello(package.Header.Seq));
        }
    }
}
