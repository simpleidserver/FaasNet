using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class DisconnectMessageHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IEnumerable<IMessageConsumer> _messageConsumers;
        private readonly IUdpClientServerFactory _udpClientFactory;

        public DisconnectMessageHandler(
            IVpnStore vpnStore,
            IEnumerable<IMessageConsumer> messageConsumers,
            IUdpClientServerFactory udpClientServerFactory) : base(vpnStore)
        {
            _udpClientFactory = udpClientServerFactory;
            _messageConsumers = messageConsumers;
        }

        public Commands Command => Commands.DISCONNECT_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var disconnectRequest = package as DisconnectRequest;
            var sessionResult = await GetActiveSession(package, disconnectRequest.ClientId, disconnectRequest.SessionId, cancellationToken);
            await CloseRemoteSessions(sessionResult.Client, disconnectRequest);
            await CloseLocalSession(sessionResult.Client, disconnectRequest.SessionId, sessionResult.Vpn, cancellationToken);
            return PackageResponseBuilder.Disconnect(package.Header.Seq);
        }

        private async Task CloseRemoteSessions(Client client, DisconnectRequest disconnectRequest)
        {
            var udpClient = _udpClientFactory.Build();
            var activeSession = client.GetActiveSession(disconnectRequest.SessionId);
            foreach (var bridgeServer in activeSession.Bridges)
            {
                var runtimeClient = new RuntimeClient(udpClient, bridgeServer.Urn, bridgeServer.Port);
                await runtimeClient.Disconnect(disconnectRequest.ClientId, bridgeServer.SessionId, true);
            }
        }

        private async Task CloseLocalSession(Client client, string sessionId, Vpn vpn, CancellationToken cancellationToken)
        {
            var activeSession = client.GetActiveSession(sessionId);
            foreach (var topic in activeSession.Topics)
            {
                var messageConsumer = _messageConsumers.First(m => m.BrokerName == topic.BrokerName);
                await messageConsumer.Unsubscribe(topic.Name, client, sessionId, CancellationToken.None);
            }

            client.CloseActiveSession(sessionId);
            VpnStore.Update(vpn);
            await VpnStore.SaveChanges(cancellationToken);
        }
    }
}