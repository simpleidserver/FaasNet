using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Extensions;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
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
        private readonly RuntimeOptions _options;

        public AsyncMessageToServerHandler(
            IClientStore clientStore,
            IUdpClientServerFactory udpClientFactory,
            IOptions<RuntimeOptions> options)
        {
            _clientStore = clientStore;
            _udpClientFactory = udpClientFactory;
            _options = options.Value;
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

            var lastBridge = pkg.BridgeServers.Last();
            var activeSession = client.GetActiveSession(pkg.SessionId, lastBridge.Urn);
            if (activeSession == null)
            {
                throw new RuntimeException(pkg.Header.Command, pkg.Header.Seq, Errors.NO_ACTIVE_SESSION);
            }

            var writeCtx = new WriteBufferContext();
            var udpClient = _udpClientFactory.Build();
            switch (activeSession.Type)
            {
                case Models.ClientSessionTypes.SERVER:
                    var bridgeServers = pkg.BridgeServers;
                    bridgeServers.Add(new AsyncMessageBridgeServer { Port = _options.Port, Urn = _options.Urn });
                    PackageResponseBuilder.AsyncMessageToServer(pkg.ClientId, bridgeServers, pkg.BrokerName, pkg.Topic, pkg.CloudEvents, pkg.SessionId).Serialize(writeCtx);
                    break;
                case Models.ClientSessionTypes.CLIENT:
                    PackageResponseBuilder.AsyncMessageToClient(pkg.BridgeServers, pkg.BrokerName, pkg.Topic, pkg.CloudEvents).Serialize(writeCtx);
                    break;
            }

            var payload = writeCtx.Buffer.ToArray();
            await udpClient.SendAsync(payload, payload.Count(), activeSession.Endpoint).WithCancellation(cancellationToken);
            return null;
        }
    }
}
