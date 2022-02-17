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
    public class PublishMessageRequestHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IUdpClientServerFactory _udpClientFactory;
        private readonly IBridgeServerStore _bridgeServerStore;
        private readonly IEnumerable<IMessagePublisher> _messagePublishers;
        private readonly RuntimeOptions _runtimeOpts;

        public PublishMessageRequestHandler(
            IUdpClientServerFactory udpClientServerFactory,
            IBridgeServerStore bridgeServerStore,
            IClientStore clientStore, 
            IEnumerable<IMessagePublisher> messagePublishers,
            IOptions<RuntimeOptions> runtimeOpts) : base(clientStore)
        {
            _udpClientFactory = udpClientServerFactory;
            _bridgeServerStore = bridgeServerStore;
            _messagePublishers = messagePublishers;
            _runtimeOpts = runtimeOpts.Value;
        }

        public Commands Command => Commands.PUBLISH_MESSAGE_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var publishMessageRequest = package as PublishMessageRequest;
            var client = GetActiveSession(publishMessageRequest, publishMessageRequest.ClientId, publishMessageRequest.SessionId);
            var activeSession = client.GetActiveSession(publishMessageRequest.SessionId);
            if (activeSession.Purpose != UserAgentPurpose.PUB)
            {
                throw new RuntimeException(publishMessageRequest.Header.Command, publishMessageRequest.Header.Seq, Errors.UNAUTHORIZED_PUBLISH);
            }

            if(
                (!string.IsNullOrWhiteSpace(publishMessageRequest.Urn) && _runtimeOpts.Urn == publishMessageRequest.Urn) ||
                (string.IsNullOrWhiteSpace(publishMessageRequest.Urn)))
            {
                foreach(var publisher in _messagePublishers)
                {
                    await publisher.Publish(publishMessageRequest.CloudEvent, publishMessageRequest.Topic, client);
                }
            }

            await Broadcast(publishMessageRequest, client);
            return PackageResponseBuilder.PublishMessage(package.Header.Seq);
        }

        private async Task Broadcast(PublishMessageRequest publishMessageRequest, Client client)
        {
            foreach (var bridgeServer in _bridgeServerStore.GetAll())
            {
                await Broadcast(publishMessageRequest, bridgeServer, client);
            }
        }

        private async Task Broadcast(PublishMessageRequest publishMessageRequest, BridgeServer bridgeServer, Client client)
        {
            var activeSession = client.GetActiveSession(publishMessageRequest.SessionId);
            var pid = Process.GetCurrentProcess().Id;
            var runtimeClient = new RuntimeClient(bridgeServer.Urn, bridgeServer.Port);
            var helloResponse = await runtimeClient.Hello(new UserAgent
            {
                ClientId = client.ClientId,
                Purpose = activeSession.Purpose,
                Environment = activeSession.Environment,
                BufferCloudEvents = activeSession.BufferCloudEvents,
                Urn = _runtimeOpts.Urn,
                Port = _runtimeOpts.Port,
                Pid = pid,
                IsServer = true
            });
            await runtimeClient.PublishMessage(client.ClientId, helloResponse.SessionId, publishMessageRequest.Topic, publishMessageRequest.CloudEvent, publishMessageRequest.Urn, publishMessageRequest.Port);
            await runtimeClient.Disconnect(client.ClientId, helloResponse.SessionId);
        }
    }
}
