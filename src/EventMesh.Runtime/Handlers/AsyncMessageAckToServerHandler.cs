using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Stores;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class AsyncMessageAckToServerHandler : IMessageHandler
    {
        private readonly IClientStore _clientStore;

        public AsyncMessageAckToServerHandler(IClientStore clientStore)
        {
            _clientStore = clientStore;
        }

        public Commands Command => Commands.ASYNC_MESSAGE_TO_CLIENT_ACK;

        public Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var client = _clientStore.GetByActiveSession(sender);
            if (client == null)
            {
                throw new RuntimeException(package.Header.Command, package.Header.Seq, Errors.NO_ACTIVE_SESSION);
            }

            var ackResponse = package as AsyncMessageAckToServer;
            client.ConsumeCloudEvents(ackResponse.BrokerName, ackResponse.Topic, ackResponse.NbCloudEventsConsumed);
            _clientStore.Update(client);
            return Task.FromResult((Package)null);
        }
    }
}
