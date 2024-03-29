﻿using FaasNet.EventMesh.Client.Extensions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class PublishMessageRequestHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IClusterStore _clusterStore;
        private readonly ConsensusNodeOptions _nodeOptions;

        public PublishMessageRequestHandler(IVpnStore vpnStore, IClientSessionStore clientSessionStore, IClusterStore clusterStore, IOptions<ConsensusNodeOptions> nodeOptions) : base(clientSessionStore, vpnStore)
        {
            _clusterStore = clusterStore;
            _nodeOptions = nodeOptions.Value;
        }

        public Commands Command => Commands.PUBLISH_MESSAGE_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var publishMessageRequest = package as PublishMessageRequest;
            var cloudEvent = publishMessageRequest.CloudEvent;
            cloudEvent.Type = publishMessageRequest.Topic;
            await CheckSession(publishMessageRequest, cancellationToken);
            var result = PackageResponseBuilder.PublishMessage(package.Header.Seq);
            if (CheckPeerExists(peers, publishMessageRequest.Topic))
            {
                await SendMessage(publishMessageRequest, cancellationToken);
                return EventMeshPackageResult.SendResult(result);
            }

            var base64Message = cloudEvent.SerializeBase64();
            return EventMeshPackageResult.AddPeer(publishMessageRequest.Topic, result, new LogRecord
            {
                Index = 1,
                Value = base64Message,
                InsertionDateTime = DateTime.UtcNow
            });
        }

        private async Task CheckSession(PublishMessageRequest message, CancellationToken cancellationToken)
        {
            ActiveSessionResult sessionResult = null;
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get active session"))
            {
                sessionResult = await GetActiveSession(message, message.SessionId, cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            if (sessionResult.ClientSession.Purpose != UserAgentPurpose.PUB) throw new RuntimeException(message.Header.Command, message.Header.Seq, Errors.UNAUTHORIZED_PUBLISH);
            if (!sessionResult.ClientSession.IsActive) throw new RuntimeException(message.Header.Command, message.Header.Seq, Errors.NO_ACTIVE_SESSION);
        }

        private bool CheckPeerExists(IEnumerable<IPeerHost> peers, string topicName)
        {
            var peer = peers.FirstOrDefault(p => p.Info.TermId == topicName);
            return peer != null;
        }

        private async Task SendMessage(PublishMessageRequest message, CancellationToken cancellationToken)
        {
            var rndClusterNode = await GetRandomClusterNode(cancellationToken);
            var base64Message = message.CloudEvent.SerializeBase64();
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Broadcast message"))
            {
                using (var consensusClient = new ConsensusClient(rndClusterNode.Url, rndClusterNode.Port))
                {
                    await consensusClient.AppendEntry(message.Topic, base64Message, cancellationToken);
                }
            }
        }

        private async Task<ClusterNode> GetRandomClusterNode(CancellationToken cancellationToken)
        {
            var nodes = await _clusterStore.GetAllNodes(cancellationToken);
            if (nodes.Count() == 1) return nodes.First();
            nodes = nodes.Where(n => n.Port != _nodeOptions.ExposedPort && n.Url != _nodeOptions.ExposedUrl);
            var rnd = new Random();
            var rndIndex = rnd.Next(0, nodes.Count() - 1);
            return nodes.ElementAt(rndIndex);
        }
    }
}
