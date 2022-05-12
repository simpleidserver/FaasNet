using FaasNet.EventMesh.Client.Extensions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Models;
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

        public async Task<EventMeshPackageResult> Run(Package package, CancellationToken cancellationToken)
        {
            var publishMessageRequest = package as PublishMessageRequest;
            await CheckSession(publishMessageRequest, cancellationToken);
            var queueNames = await GetQueueNames(publishMessageRequest, cancellationToken);
            await BroadcastMessage(publishMessageRequest, queueNames, cancellationToken);
            var result = PackageResponseBuilder.PublishMessage(package.Header.Seq);
            return EventMeshPackageResult.SendResult(result);
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

        private async Task<IEnumerable<string>> GetQueueNames(PublishMessageRequest message, CancellationToken cancellationToken)
        {
            IEnumerable<MessageExchange> messageExchanges;
            using(var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get all message exchange"))
            {
                messageExchanges = await _messageExchangeStore.GetAll(cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            var queueNames = new List<string>();
            foreach(var messageExchange in messageExchanges)
            {
                if (messageExchange.IsMatch(message.Topic)) queueNames.AddRange(messageExchange.ClientIds.Select(ci => Models.Client.BuildQueueName((ci))));
            }

            return queueNames;
        }

        private async Task BroadcastMessage(PublishMessageRequest message, IEnumerable<string> queueNames, CancellationToken cancellationToken)
        {
            var rndClusterNode = await GetRandomClusterNode(cancellationToken);
            var base64Message = message.CloudEvent.SerializeBase64();
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Broadcast message"))
            {
                using (var consensusClient = new ConsensusClient(rndClusterNode.Url, rndClusterNode.Port))
                {
                    foreach (var queueName in queueNames)
                    {
                        await consensusClient.AppendEntry(queueName, base64Message, cancellationToken);
                    }
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
