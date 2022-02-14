using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class AsyncMessageToClientAckHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IBridgeServerStore _bridgeServerStore;
        private readonly IUdpClientServerFactory _udpClientFactory;

        public AsyncMessageToClientAckHandler(
            IClientStore clientStore,
            IBridgeServerStore bridgeServerStore,
            IUdpClientServerFactory udpClientServerFactory) : base(clientStore) 
        {
            _bridgeServerStore = bridgeServerStore;
            _udpClientFactory = udpClientServerFactory;
        }

        public Commands Command => Commands.ASYNC_MESSAGE_TO_CLIENT_ACK;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var ackResponse = package as AsyncMessageAckToServer;
            var client = GetActiveSession(ackResponse, ackResponse.ClientId, ackResponse.SessionId);
            if (ConsumeCloudEvents(ackResponse, client))
            {
                return PackageResponseBuilder.AsyncMessageToClient(package.Header.Seq);
            }

            return await TransmitCloudEvents(ackResponse, client);
        }

        private bool ConsumeCloudEvents(AsyncMessageAckToServer ackResponse, Client client)
        {
            if (ackResponse.BridgeServers.Any())
            {
                return false;
            }

            client.ConsumeCloudEvents(ackResponse.BrokerName, ackResponse.Topic, ackResponse.NbCloudEventsConsumed);
            ClientStore.Update(client);
            return true;
        }

        private async Task<Package> TransmitCloudEvents(AsyncMessageAckToServer ackResponse, Client client)
        {
            var bridgeServers = _bridgeServerStore.GetAll();
            var lastBridgeServer = ackResponse.BridgeServers.Last();
            if(!bridgeServers.Any(bs => bs.Port == lastBridgeServer.Port && bs.Urn == lastBridgeServer.Urn))
            {
                throw new RuntimeException(ackResponse.Header.Command, ackResponse.Header.Seq, Errors.UNKNOWN_BRIDGE);
            }

            var activeSession = client.GetActiveSession(ackResponse.SessionId);
            var bridgeSessionId = activeSession.GetBridge(lastBridgeServer.Urn).SessionId;
            var udpClient = _udpClientFactory.Build();
            var runtimeClient = new RuntimeClient(udpClient, lastBridgeServer.Urn, lastBridgeServer.Port);
            ackResponse.BridgeServers.Remove(lastBridgeServer);
            return await runtimeClient.TransferMessageToServer(ackResponse.ClientId, ackResponse.BrokerName, ackResponse.Topic, ackResponse.NbCloudEventsConsumed, ackResponse.BridgeServers, bridgeSessionId);
        }
    }
}
