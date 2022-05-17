using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Extensions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.RaftConsensus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class ReadTopicMessageHandler : IMessageHandler
    {
        public Commands Command => Commands.READ_TOPIC_MESSAGE_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var readMessage = package as ReadMessageTopicRequest;
            var message = await ReadMessage(readMessage, peers, cancellationToken);
            CloudEvent cloudEvt = null;
            if(!string.IsNullOrWhiteSpace(message)) cloudEvt = Convert.FromBase64String(message).DeserializeCloudEvent();
            var result = PackageResponseBuilder.ReadTopicMessage(readMessage.Topic, cloudEvt, readMessage.EvtOffset, readMessage.Header.Seq);
            return EventMeshPackageResult.SendResult(result);
        }

        private async Task<string> ReadMessage(ReadMessageTopicRequest request, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var selectedPeer = peers.FirstOrDefault(p => p.Info.TermId == request.Topic);
            if (selectedPeer == null) return null;
            var record = await selectedPeer.ReadRecord(request.EvtOffset, cancellationToken);
            return record?.Value;
        }
    }
}
