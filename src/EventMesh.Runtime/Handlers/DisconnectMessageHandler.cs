using EventMesh.Runtime.Messages;
using EventMesh.Runtime.Stores;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class DisconnectMessageHandler : BaseMessageHandler, IMessageHandler
    {
        public DisconnectMessageHandler(IClientStore clientStore) : base(clientStore)
        {
        }

        public Commands Command => Commands.DISCONNECT_REQUEST;

        public Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var client = GetActiveSession(package, sender);
            client.CloseActiveSession();
            ClientStore.Update(client);
            return Task.FromResult(PackageResponseBuilder.Disconnect(package.Header.Seq));
        }
    }
}
