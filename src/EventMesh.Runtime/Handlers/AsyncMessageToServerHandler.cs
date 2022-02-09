using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Extensions;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Stores;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class AsyncMessageToServerHandler : IMessageHandler
    {
        private readonly IClientStore _clientStore;
        private readonly IUdpClientServerFactory _udpClientFactory;

        public AsyncMessageToServerHandler(
            IClientStore clientStore,
            IUdpClientServerFactory udpClientFactory)
        {
            _clientStore = clientStore;
            _udpClientFactory = udpClientFactory;
        }

        public Commands Command => Commands.ASYNC_MESSAGE_TO_SERVER;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var pkg = package as AsyncMessageToServer;
            var client = _clientStore.Get(pkg.ClientId);
            if (client == null)
            {
                throw new RuntimeException(pkg.Header.Command, pkg.Header.Seq, Errors.INVALID_CLIENT);
            }

            if (client.ActiveSession == null)
            {
                throw new RuntimeException(pkg.Header.Command, pkg.Header.Seq, Errors.NO_ACTIVE_SESSION);
            }

            var writeCtx = new WriteBufferContext();
            var udpClient = _udpClientFactory.Build();
            switch(client.ActiveSession.Type)
            {
                case Models.ClientSessionTypes.SERVER:
                    PackageResponseBuilder.AsyncMessageToServer(pkg.ClientId, pkg.Urn, pkg.BrokerName, pkg.Topic, pkg.CloudEvents, pkg.Header.Seq).Serialize(writeCtx);
                    break;
                case Models.ClientSessionTypes.CLIENT:
                    PackageResponseBuilder.AsyncMessageToClient(pkg.Urn, pkg.BrokerName, pkg.Topic, pkg.CloudEvents, pkg.Header.Seq).Serialize(writeCtx);
                    break;
            }

            var payload = writeCtx.Buffer.ToArray();
            await udpClient.SendAsync(payload, payload.Count(), client.ActiveSession.Endpoint).WithCancellation(cancellationToken);
            return null;
        }
    }
}
