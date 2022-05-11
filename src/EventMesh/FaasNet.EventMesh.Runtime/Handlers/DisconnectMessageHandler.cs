using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class DisconnectMessageHandler : BaseMessageHandler, IMessageHandler
    {
        public DisconnectMessageHandler(IClientSessionStore clientSessionStore, IVpnStore vpnStore) : base(clientSessionStore, vpnStore)
        {
        }

        public Commands Command => Commands.DISCONNECT_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, CancellationToken cancellationToken)
        {
            var disconnectRequest = package as DisconnectRequest;
            var sessionResult = await GetActiveSession(package, disconnectRequest.SessionId, cancellationToken);
            // await CloseRemoteSessions(sessionResult.Client, disconnectRequest);
            // await CloseLocalSession(sessionResult.ClientSession, disconnectRequest.SessionId, sessionResult.Vpn, cancellationToken);
            // return PackageResponseBuilder.Disconnect(package.Header.Seq);
            return null;
        }

        /*
        private async Task CloseRemoteSessions(Models.Client client, DisconnectRequest disconnectRequest)
        {
            var udpClient = _udpClientFactory.Build();
            var activeSession = client.GetActiveSession(disconnectRequest.SessionId);
            foreach (var bridgeServer in activeSession.Bridges)
            {
                var runtimeClient = new RuntimeClient(udpClient, bridgeServer.Urn, bridgeServer.Port);
                await runtimeClient.Disconnect(disconnectRequest.ClientId, bridgeServer.SessionId, true);
            }
        }
        */

        /*
        private Task CloseLocalSession(Models.Client client, string sessionId, Vpn vpn, CancellationToken cancellationToken)
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
            return Task.CompletedTask;
        }
        */
    }
}