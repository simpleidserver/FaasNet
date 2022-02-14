using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;

namespace EventMesh.Runtime.Handlers
{
    public class BaseMessageHandler
    {
        public BaseMessageHandler(IClientStore clientStore)
        {
            ClientStore = clientStore;
        }

        protected IClientStore ClientStore { get; private set; }

        public Client GetActiveSession(Package requestPackage, string clientId, string sessionId)
        {
            var client = ClientStore.GetByActiveSession(clientId, sessionId);
            if (client == null)
            {
                throw new RuntimeException(requestPackage.Header.Command, requestPackage.Header.Seq, Errors.INVALID_CLIENT);
            }

            return client;
        }
    }
}
