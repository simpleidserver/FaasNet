using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class SubscribeMessageHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly IEnumerable<IMessageConsumer> _messageConsumers;
        private readonly RuntimeOptions _options;

        public SubscribeMessageHandler(
            IVpnStore vpnStore,
            IUdpClientServerFactory udpClientFactory,
            IEnumerable<IMessageConsumer> messageConsumers,
            IOptions<RuntimeOptions> options) : base(vpnStore)
        {
            _udpClientFactory = udpClientFactory;
            _messageConsumers = messageConsumers;
            _options = options.Value;
        }

        public Commands Command => Commands.SUBSCRIBE_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var subscriptionRequest = package as SubscriptionRequest;
            var sessionResult = await GetActiveSession(package, subscriptionRequest.ClientId, subscriptionRequest.SessionId, cancellationToken);
            var activeSession = sessionResult.Client.GetActiveSession(subscriptionRequest.SessionId);
            if (activeSession.Purpose != UserAgentPurpose.SUB)
            {
                throw new RuntimeException(subscriptionRequest.Header.Command, subscriptionRequest.Header.Seq, Errors.UNAUTHORIZED_SUBSCRIBE);
            }

            await SubscribeBridgeServerTopics(subscriptionRequest, sessionResult.Client, sessionResult.Vpn.BridgeServers);
            await Subscribe(subscriptionRequest, sessionResult.Client);
            VpnStore.Update(sessionResult.Vpn);
            await VpnStore.SaveChanges(cancellationToken);
            return PackageResponseBuilder.Subscription(package.Header.Seq);
        }

        private async Task Subscribe(SubscriptionRequest subscriptionRequest, Client client)
        {
            Thread.Sleep(_options.WaitLocalSubscriptionIntervalMS);
            await SubscribeLocalTopics(subscriptionRequest, client, CancellationToken.None);
        }

        #region Register Local Topics

        private async Task SubscribeLocalTopics(SubscriptionRequest subscriptionRequest, Client client, CancellationToken cancellationToken)
        {
            foreach (var item in subscriptionRequest.Topics)
            {
                foreach (var messageConsumer in _messageConsumers)
                {
                    await messageConsumer.Subscribe(item.Topic, client, subscriptionRequest.SessionId, cancellationToken);
                }
            }
        }

        #endregion

        #region Register Topics from BridgeServer

        private async Task SubscribeBridgeServerTopics(SubscriptionRequest subscriptionRequest, Client client, ICollection<BridgeServer> bridgeServers)
        {
            foreach (var bridgeServer in bridgeServers)
            {
                await SubscribeBridgeServer(client, bridgeServer, subscriptionRequest);
            }
        }

        private async Task SubscribeBridgeServer(Client client, BridgeServer bridgeServer, SubscriptionRequest subscriptionRequest)
        {
            var activeSession = client.GetActiveSession(subscriptionRequest.SessionId);
            var udpClient = _udpClientFactory.Build();
            var pid = Process.GetCurrentProcess().Id;
            var bridge = activeSession.GetBridge(bridgeServer.Urn, bridgeServer.Port, bridgeServer.Vpn);
            var runtimeClient = new RuntimeClient(udpClient, bridgeServer.Urn, bridgeServer.Port);
            if (bridge == null)
            {
                var helloResponse = await runtimeClient.Hello(new UserAgent
                {
                    ClientId = client.ClientId,
                    Purpose = activeSession.Purpose,
                    Environment = activeSession.Environment,
                    BufferCloudEvents = activeSession.BufferCloudEvents,
                    Urn = _options.Urn,
                    Port = _options.Port,
                    Pid = pid,
                    IsServer = true,
                    Vpn = bridgeServer.Vpn
                });
                bridge = ClientSessionBridge.Create(bridgeServer.Urn, bridgeServer.Port, helloResponse.SessionId, bridgeServer.Vpn);
                activeSession.AddBridge(bridge);
            }

            await runtimeClient.Subscribe(client.ClientId, bridge.SessionId, subscriptionRequest.Topics);
        }

        #endregion
    }
}