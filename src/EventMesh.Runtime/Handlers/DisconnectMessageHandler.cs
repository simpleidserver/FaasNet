using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class DisconnectMessageHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IBridgeServerStore _bridgeServerStore;
        private readonly IUdpClientServerFactory _udpClientFactory;

        public DisconnectMessageHandler(
            IClientStore clientStore,
            IBridgeServerStore bridgeServerStore,
            IUdpClientServerFactory udpClientServerFactory) : base(clientStore)
        {
            _bridgeServerStore = bridgeServerStore;
            _udpClientFactory = udpClientServerFactory;
        }

        public Commands Command => Commands.DISCONNECT_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var disconnectRequest = package as DisconnectRequest;
            var client = GetActiveSession(package, disconnectRequest.ClientId, disconnectRequest.SessionId);
            await CloseRemoteSessions(client, disconnectRequest);
            CloseLocalSession(client, disconnectRequest.SessionId);
            return PackageResponseBuilder.Disconnect(package.Header.Seq);
        }

        private void CloseLocalSession(Client client, string sessionId)
        {
            client.CloseActiveSession(sessionId);
            ClientStore.Update(client);
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
    }
}