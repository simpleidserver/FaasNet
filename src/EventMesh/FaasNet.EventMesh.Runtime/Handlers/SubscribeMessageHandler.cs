using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class SubscribeMessageHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly IMessageExchangeStore _messageExchangeStore;

        public SubscribeMessageHandler(IVpnStore vpnStore, IClientSessionStore clientSessionStore, IMessageExchangeStore messageExchangeStore) : base(clientSessionStore, vpnStore) 
        {
            _messageExchangeStore = messageExchangeStore;
        }

        public Commands Command => Commands.SUBSCRIBE_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var subscriptionRequest = package as SubscriptionRequest;
            var activeSession = await CheckSession(subscriptionRequest, cancellationToken);
            await AddMessageExchanges(activeSession.ClientSession.Queue, subscriptionRequest, cancellationToken);
            var result = PackageResponseBuilder.Subscription(package.Header.Seq, activeSession.ClientSession.Queue);
            return EventMeshPackageResult.AddPeer(activeSession.ClientSession.Queue, result);
        }

        private async Task<ActiveSessionResult> CheckSession(SubscriptionRequest request, CancellationToken cancellationToken)
        {
            ActiveSessionResult sessionResult = null;
            using (var activity = EventMeshMeter.RequestActivitySource.StartActivity("Get active session"))
            {
                sessionResult = await GetActiveSession(request, request.SessionId, cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            if (sessionResult.ClientSession.Purpose != UserAgentPurpose.SUB)
            {
                throw new RuntimeException(request.Header.Command, request.Header.Seq, Errors.UNAUTHORIZED_SUBSCRIBE);
            }

            return sessionResult;
        }

        private async Task AddMessageExchanges(string queueName, SubscriptionRequest subscriptionRequest, CancellationToken cancellationToken)
        {
            foreach (var topicFilter in subscriptionRequest.TopicFilters)
            {
                var messageExchange = await _messageExchangeStore.Get(topicFilter.Topic, cancellationToken);
                if (messageExchange == null) messageExchange = new Models.MessageExchange { TopicFilter = topicFilter.Topic };
                if (!messageExchange.QueueNames.Contains(queueName)) messageExchange.QueueNames.Add(queueName);
                await _messageExchangeStore.Add(messageExchange, cancellationToken);
            }
        }
    }
}