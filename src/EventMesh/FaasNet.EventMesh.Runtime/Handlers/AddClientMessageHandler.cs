using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class AddClientMessageHandler : IMessageHandler
    {
        private readonly IClientStore _clientStore;

        public AddClientMessageHandler(IClientStore clientStore)
        {
            _clientStore = clientStore;
        }

        public Commands Command => Commands.ADD_CLIENT_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, CancellationToken cancellationToken)
        {
            var addClient = package as AddClientRequest;
            var client = Models.Client.Create(addClient.Vpn, addClient.ClientId, null, null);
            await _clientStore.Add(client, cancellationToken);
            return EventMeshPackageResult.AddPeer(client.Queue, PackageResponseBuilder.Client(package.Header.Seq, client.Queue));
        }
    }
}
