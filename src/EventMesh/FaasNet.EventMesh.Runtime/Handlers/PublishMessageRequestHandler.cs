using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class PublishMessageRequestHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IMessageExchangeStore _messageExchangeStore;

        public PublishMessageRequestHandler(IVpnStore vpnStore, IClientSessionStore clientSessionStore, IMessageExchangeStore messageExchangeStore) : base(clientSessionStore, vpnStore)
        {
            _messageExchangeStore = messageExchangeStore;
        }

        public Commands Command => Commands.PUBLISH_MESSAGE_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, CancellationToken cancellationToken)
        {
            var publishMessageRequest = package as PublishMessageRequest;
            await CheckSession(publishMessageRequest, cancellationToken);

            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Publish to local message brokers"))
            {
                if (
                    (!string.IsNullOrWhiteSpace(publishMessageRequest.Urn) && _runtimeOpts.Urn == publishMessageRequest.Urn) ||
                    (string.IsNullOrWhiteSpace(publishMessageRequest.Urn)))
                {
                    foreach (var publisher in _messagePublishers)
                    {
                        await publisher.Publish(publishMessageRequest.CloudEvent, publishMessageRequest.Topic, sessionResult.ClientSession);
                    }
                }

                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Broadcast message"))
            {
                await Broadcast(publishMessageRequest, sessionResult.ClientSession, sessionResult.Vpn.BridgeServers);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            return PackageResponseBuilder.PublishMessage(package.Header.Seq);
        }

        private async Task CheckSession(PublishMessageRequest message, CancellationToken cancellationToken)
        {
            ActiveSessionResult sessionResult = null;
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get active session"))
            {
                sessionResult = await GetActiveSession(message, message.SessionId, cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            if (sessionResult.ClientSession.Purpose != UserAgentPurpose.PUB)
            {
                throw new RuntimeException(message.Header.Command, message.Header.Seq, Errors.UNAUTHORIZED_PUBLISH);
            }
        }

        private async Task BroadcastMessage(PublishMessageRequest message, CancellationToken cancellationToken)
        {
            // get all the queues from exchange.
            // append entry | append the message to the queue.
            // a client must pool the message from the queue (consensus).
        }

        private async Task Broadcast(PublishMessageRequest publishMessageRequest, Models.Client client, ICollection<BridgeServer> bridgeServers)
        {
            foreach (var bridgeServer in bridgeServers)
            {
                await Broadcast(publishMessageRequest, bridgeServer, client);
            }
        }

        private async Task Broadcast(PublishMessageRequest publishMessageRequest, BridgeServer bridgeServer, Models.Client client)
        {
            var activeSession = client.GetActiveSession(publishMessageRequest.SessionId);
            var pid = Process.GetCurrentProcess().Id;
            var runtimeClient = new RuntimeClient(bridgeServer.TargetUrn, bridgeServer.TargetPort);
            var helloResponse = await runtimeClient.Hello(new UserAgent
            {
                ClientId = client.ClientId,
                Purpose = activeSession.Purpose,
                Environment = activeSession.Environment,
                BufferCloudEvents = activeSession.BufferCloudEvents,
                Urn = _runtimeOpts.Urn,
                Port = _runtimeOpts.Port,
                Pid = pid,
                IsServer = true,
                Vpn = bridgeServer.TargetVpn
            });
            await runtimeClient.PublishMessage(client.ClientId, helloResponse.SessionId, publishMessageRequest.Topic, publishMessageRequest.CloudEvent, publishMessageRequest.Urn, publishMessageRequest.Port);
            await runtimeClient.Disconnect(client.ClientId, helloResponse.SessionId);
        }
    }
}
