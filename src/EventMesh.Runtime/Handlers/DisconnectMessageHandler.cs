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
            var client = GetActiveSession(package, disconnectRequest.ClientId, sender);
            CloseLocalSession(client);
            await CloseRemoteSessions(disconnectRequest);
            return PackageResponseBuilder.Disconnect(package.Header.Seq);
        }

        private void CloseLocalSession(Client client)
        {
            client.CloseActiveSession();
            ClientStore.Update(client);
        }

        private async Task CloseRemoteSessions(DisconnectRequest disconnectRequest)
        {
            var udpClient = _udpClientFactory.Build();
            foreach (var bridgeServer in _bridgeServerStore.GetAll())
            {
                var runtimeClient = new RuntimeClient(udpClient, bridgeServer.Urn, bridgeServer.Port);
                await runtimeClient.Disconnect(disconnectRequest.ClientId, true);
            }
        }
    }
}