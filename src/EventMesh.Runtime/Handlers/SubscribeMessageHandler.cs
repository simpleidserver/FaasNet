using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class SubscribeMessageHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IBridgeServerStore _bridgeServerStore;
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly IEnumerable<IMessageConsumer> _messageConsumers;
        private readonly RuntimeOptions _options;

        public SubscribeMessageHandler(
            IClientStore clientStore,
            IBridgeServerStore bridgeServerStore,
            IUdpClientServerFactory udpClientFactory,
            IEnumerable<IMessageConsumer> messageConsumers,
            IOptions<RuntimeOptions> options) : base(clientStore)
        {
            _bridgeServerStore = bridgeServerStore;
            _udpClientFactory = udpClientFactory;
            _messageConsumers = messageConsumers;
            _options = options.Value;
        }

        public Commands Command => Commands.SUBSCRIBE_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var subscriptionRequest = package as SubscriptionRequest;
            var client = GetActiveSession(package, subscriptionRequest.ClientId, subscriptionRequest.SessionId);
            var activeSession = client.GetActiveSession(subscriptionRequest.SessionId);
            if (activeSession.Purpose != UserAgentPurpose.SUB)
            {
                throw new RuntimeException(subscriptionRequest.Header.Command, subscriptionRequest.Header.Seq, Errors.UNAUTHORIZED_SUBSCRIBE);
            }

            await SubscribeBridgeServerTopics(subscriptionRequest, client);
            await Subscribe(subscriptionRequest, client);
            ClientStore.Update(client);
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

        private async Task SubscribeBridgeServerTopics(SubscriptionRequest subscriptionRequest, Client client)
        {
            var bridgeServers = _bridgeServerStore.GetAll();
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
            var bridge = activeSession.GetBridge(bridgeServer.Urn);
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
                    IsServer = true
                });
                bridge = ClientSessionBridge.Create(bridgeServer.Urn, bridgeServer.Port, helloResponse.SessionId);
                activeSession.AddBridge(bridge);
            }

            await runtimeClient.Subscribe(client.ClientId, bridge.SessionId, subscriptionRequest.Topics);
        }

        #endregion
    }
}