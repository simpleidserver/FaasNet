using EventMesh.Runtime.Exceptions;
using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using System.Net;

namespace EventMesh.Runtime.Handlers
{
    public class BaseMessageHandler
    {
        public BaseMessageHandler(IClientStore clientStore)
        {
            ClientStore = clientStore;
        }

        protected IClientStore ClientStore { get; private set; }

        public Client GetActiveSession(Package requestPackage, string clientId, IPEndPoint ipEndpoint)
        {
            var client = ClientStore.GetByActiveSession(clientId, ipEndpoint);
            if (client == null)
            {
                throw new RuntimeException(requestPackage.Header.Command, requestPackage.Header.Seq, Errors.INVALID_CLIENT);
            }

            return client;
        }
    }
}
