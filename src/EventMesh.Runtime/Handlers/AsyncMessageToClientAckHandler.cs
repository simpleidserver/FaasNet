using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Stores;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class AsyncMessageToClientAckHandler : BaseMessageHandler, IMessageHandler
    {
        public AsyncMessageToClientAckHandler(IClientStore clientStore) : base(clientStore) { }

        public Commands Command => Commands.ASYNC_MESSAGE_TO_CLIENT_ACK;

        public Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            /*
            var ackResponse = package as AsyncMessageAckToServer;
            var client = GetActiveSession(ackResponse, sender);
            client.ConsumeCloudEvents(ackResponse.BrokerName, ackResponse.Topic, ackResponse.NbCloudEventsConsumed);
            ClientStore.Update(client);
            return Task.FromResult((Package)null);
            */
            return Task.FromResult((Package)null);
        }
    }
}
