using FaasNet.EventMesh.Runtime.Exceptions;
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
    public class AsyncMessageToClientAckHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly IEnumerable<IMessageConsumer> _messageConsumers;

        public AsyncMessageToClientAckHandler(
            IClientStore clientStore,
            IVpnStore vpnStore,
            IUdpClientServerFactory udpClientServerFactory,
            IEnumerable<IMessageConsumer> messageConsumers) : base(clientStore, vpnStore) 
        {
            _udpClientFactory = udpClientServerFactory;
            _messageConsumers = messageConsumers;
        }

        public Commands Command => Commands.ASYNC_MESSAGE_TO_CLIENT_ACK;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            Package result = null;
            var ackResponse = package as AsyncMessageAckToServer;
            var sessionResult = await GetActiveSession(ackResponse, ackResponse.ClientId, ackResponse.SessionId, cancellationToken);
            if (await ConsumeCloudEvents(ackResponse, sessionResult.Client, sessionResult.Vpn, cancellationToken))
            {
                result = PackageResponseBuilder.AsyncMessageToClient(package.Header.Seq);
            }
            else
            {
                result = await TransmitCloudEvents(ackResponse, sessionResult.Client, sessionResult.Vpn.BridgeServers);
            }

            return ackResponse.IsClient ? null : result;
        }

        private async Task<bool> ConsumeCloudEvents(AsyncMessageAckToServer ackResponse, Client client, Vpn vpn, CancellationToken cancellationToken)
        {
            if (ackResponse.BridgeServers.Any())
            {
                return false;
            }

            var messageConsumer = _messageConsumers.First(m => m.BrokerName == ackResponse.BrokerName);
            messageConsumer.Commit(ackResponse.Topic, client, ackResponse.SessionId, ackResponse.NbCloudEventsConsumed);
            client.ConsumeCloudEvents(ackResponse.BrokerName, ackResponse.Topic, ackResponse.NbCloudEventsConsumed);
            VpnStore.Update(vpn);
            await VpnStore.SaveChanges(cancellationToken);
            return true;
        }

        private async Task<Package> TransmitCloudEvents(AsyncMessageAckToServer ackResponse, Client client, ICollection<BridgeServer> bridgeServers)
        {
            var lastBridgeServer = ackResponse.BridgeServers.Last();
            if(!bridgeServers.Any(bs => bs.Port == lastBridgeServer.Port && bs.Urn == lastBridgeServer.Urn))
            {
                throw new RuntimeException(ackResponse.Header.Command, ackResponse.Header.Seq, Errors.UNKNOWN_BRIDGE);
            }

            var activeSession = client.GetActiveSession(ackResponse.SessionId);
            var bridgeSessionId = activeSession.GetBridge(lastBridgeServer.Urn, lastBridgeServer.Port, lastBridgeServer.Vpn).SessionId;
            var udpClient = _udpClientFactory.Build();
            var runtimeClient = new RuntimeClient(udpClient, lastBridgeServer.Urn, lastBridgeServer.Port);
            ackResponse.BridgeServers.Remove(lastBridgeServer);
            return await runtimeClient.TransferMessageToServerFromServer(ackResponse.ClientId, ackResponse.BrokerName, ackResponse.Topic, ackResponse.NbCloudEventsConsumed, ackResponse.BridgeServers, bridgeSessionId);
        }
    }
}
