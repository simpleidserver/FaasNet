using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Extensions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class ReadNextMessageHandler : IMessageHandler
    {
        private readonly IClientSessionStore _clientSessionStore;
        private readonly IQueueStore _queueStore;

        public ReadNextMessageHandler(IClientSessionStore clientSessionStore, IQueueStore queueStore)
        {
            _clientSessionStore = clientSessionStore;
            _queueStore = queueStore;
        }

        public Commands Command => Commands.READ_NEXT_MESSAGE_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var readMessage = package as ReadNextMessageRequest;
            await GetSession(readMessage, cancellationToken);
            var nextMessage = await ReadNextMessage(readMessage, peers, cancellationToken);
            var result = PackageResponseBuilder.ReadNextMessage(null, package.Header.Seq);
            if (nextMessage != null)
            {
                result = PackageResponseBuilder.ReadNextMessage(nextMessage, package.Header.Seq);
            }

            return EventMeshPackageResult.SendResult(result);
        }

        private async Task<ClientSession> GetSession(ReadNextMessageRequest request, CancellationToken cancellationToken)
        {
            ClientSession clientSession = null;
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get active session"))
            {
                clientSession = await _clientSessionStore.Get(request.SessionId, cancellationToken);
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
            }

            if(clientSession == null) throw new RuntimeException(request.Header.Command, request.Header.Seq, Errors.INVALID_SESSION);
            if(clientSession.Purpose != UserAgentPurpose.SUB) throw new RuntimeException(request.Header.Command, request.Header.Seq, Errors.UNAUTHORIZED_PUBLISH);
            return clientSession;
        }

        private async Task<CloudEvent> ReadNextMessage(ReadNextMessageRequest request, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Read next message"))
            {
                var lastLog = await _queueStore.Dequeue(request.GroupId, cancellationToken);
                if (string.IsNullOrWhiteSpace(lastLog)) return null;
                var cloudEvt = Convert.FromBase64String(lastLog).DeserializeCloudEvent();
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
                return cloudEvt;
            }
        }
    }
}
