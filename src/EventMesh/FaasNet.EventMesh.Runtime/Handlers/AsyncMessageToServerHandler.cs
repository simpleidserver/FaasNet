using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Extensions;
using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class AsyncMessageToServerHandler : IMessageHandler
    {
        private readonly IVpnStore _vpnStore;
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly RuntimeOptions _options;

        public AsyncMessageToServerHandler(
            IVpnStore vpnStore,
            IUdpClientServerFactory udpClientFactory,
            IOptions<RuntimeOptions> options)
        {
            _vpnStore = vpnStore;
            _udpClientFactory = udpClientFactory;
            _options = options.Value;
        }

        public Commands Command => Commands.ASYNC_MESSAGE_TO_SERVER;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var pkg = package as AsyncMessageToServer;
            var lastBridge = pkg.BridgeServers.Last();
            var vpn = await _vpnStore.Get(pkg.ClientId, lastBridge.Urn, pkg.SessionId, cancellationToken);
            if (vpn == null)
            {
                throw new RuntimeException(package.Header.Command, package.Header.Seq, Errors.NO_ACTIVE_SESSION);
            }

            var client = vpn.GetClient(pkg.ClientId);
            var activeSession = client.GetActiveSession(pkg.SessionId, lastBridge.Urn);
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
