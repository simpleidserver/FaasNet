using FaasNet.EventMesh.Client.Extensions;
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
        private readonly IMessageExchangeStore _messageExchangeStore;
        private readonly IClusterStore _clusterStore;
        private readonly ConsensusPeerOptions _peerOptions;

        public PublishMessageRequestHandler(IVpnStore vpnStore, IClientSessionStore clientSessionStore, IMessageExchangeStore messageExchangeStore, IClusterStore clusterStore, IOptions<ConsensusPeerOptions> peerOption) : base(clientSessionStore, vpnStore)
        {
            _messageExchangeStore = messageExchangeStore;
            _clusterStore = clusterStore;
            _peerOptions = peerOption.Value;
        }

        public Commands Command => Commands.PUBLISH_MESSAGE_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var publishMessageRequest = package as PublishMessageRequest;
            await CheckSession(publishMessageRequest, cancellationToken);
            var result = PackageResponseBuilder.PublishMessage(package.Header.Seq);
            if (CheckPeerExists(peers, publishMessageRequest.Topic))
            {
                await SendMessage(publishMessageRequest, cancellationToken);
                return EventMeshPackageResult.SendResult(result);
            }

            var base64Message = publishMessageRequest.CloudEvent.SerializeBase64();
            return EventMeshPackageResult.AddPeer(publishMessageRequest.Topic, result, new LogRecord
            {
                Index = 0,
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

            if (sessionResult.ClientSession.Purpose != UserAgentPurpose.PUB)
            {
                throw new RuntimeException(message.Header.Command, message.Header.Seq, Errors.UNAUTHORIZED_PUBLISH);
            }
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
            nodes = nodes.Where(n => n.Port != _peerOptions.Port || n.Url != _peerOptions.Url);
            var rnd = new Random();
            var rndIndex = rnd.Next(0, nodes.Count() - 1);
            return nodes.ElementAt(rndIndex);
        }
    }
}
