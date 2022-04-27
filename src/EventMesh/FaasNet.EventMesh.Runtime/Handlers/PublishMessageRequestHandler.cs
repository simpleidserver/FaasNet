﻿using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
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
    public class PublishMessageRequestHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IEnumerable<IMessagePublisher> _messagePublishers;
        private readonly RuntimeOptions _runtimeOpts;

        public PublishMessageRequestHandler(
            IUdpClientServerFactory udpClientServerFactory,
            IVpnStore vpnStore, 
            IClientStore clientStore,
            IEnumerable<IMessagePublisher> messagePublishers,
            IOptions<RuntimeOptions> runtimeOpts) : base(clientStore, vpnStore)
        {
            _messagePublishers = messagePublishers;
            _runtimeOpts = runtimeOpts.Value;
        }

        public Commands Command => Commands.PUBLISH_MESSAGE_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var publishMessageRequest = package as PublishMessageRequest;
            ActiveSessionResult sessionResult = null;
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get active session"))
            {
                sessionResult = await GetActiveSession(publishMessageRequest, publishMessageRequest.ClientId, publishMessageRequest.SessionId, cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            var activeSession = sessionResult.Client.GetActiveSession(publishMessageRequest.SessionId);
            if (activeSession.Purpose != UserAgentPurpose.PUB)
            {
                throw new RuntimeException(publishMessageRequest.Header.Command, publishMessageRequest.Header.Seq, Errors.UNAUTHORIZED_PUBLISH);
            }

            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Publish to local message brokers"))
            {
                if (
                    (!string.IsNullOrWhiteSpace(publishMessageRequest.Urn) && _runtimeOpts.Urn == publishMessageRequest.Urn) ||
                    (string.IsNullOrWhiteSpace(publishMessageRequest.Urn)))
                {
                    foreach (var publisher in _messagePublishers)
                    {
                        await publisher.Publish(publishMessageRequest.CloudEvent, publishMessageRequest.Topic, sessionResult.Client);
                    }
                }

                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Broadcast message"))
            {
                await Broadcast(publishMessageRequest, sessionResult.Client, sessionResult.Vpn.BridgeServers);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            return PackageResponseBuilder.PublishMessage(package.Header.Seq);
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
                IsServer = true,
                Vpn = bridgeServer.Vpn
            });
            await runtimeClient.PublishMessage(client.ClientId, helloResponse.SessionId, publishMessageRequest.Topic, publishMessageRequest.CloudEvent, publishMessageRequest.Urn, publishMessageRequest.Port);
            await runtimeClient.Disconnect(client.ClientId, helloResponse.SessionId);
        }
    }
}
