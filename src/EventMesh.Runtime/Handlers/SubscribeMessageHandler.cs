using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class SubscribeMessageHandler : IMessageHandler
    {
        private readonly IClientStore _clientSessionStore;
        private readonly IEnumerable<IMessageConsumer> _messageConsumers;

        public SubscribeMessageHandler(
            IClientStore clientSessionStore,
            IEnumerable<IMessageConsumer> messageConsumers)
        {
            _clientSessionStore = clientSessionStore;
            _messageConsumers = messageConsumers;
        }

        public Commands Command => Commands.SUBSCRIBE_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var client = _clientSessionStore.GetByActiveSession(sender);
            if (client == null)
            {
                throw new RuntimeException(package.Header.Command, package.Header.Seq, Errors.NO_ACTIVE_SESSION);
            }

            var subscriptionRequest = package as SubscriptionRequest;
            await Subscribe(subscriptionRequest, client, cancellationToken);
            _clientSessionStore.Update(client);
            return PackageResponseBuilder.Subscription(package.Header.Seq);
        }

        private async Task Subscribe(SubscriptionRequest subscriptionRequest, Client client, CancellationToken cancellationToken)
        {
            foreach(var item in subscriptionRequest.Topics)
            {
                foreach(var messageConsumer in _messageConsumers)
                {
                    await messageConsumer.Subscribe(item.Topic, client, cancellationToken);
                }
            }
        }
    }
}