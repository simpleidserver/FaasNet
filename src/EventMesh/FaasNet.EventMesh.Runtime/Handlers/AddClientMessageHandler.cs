using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var addClient = package as AddClientRequest;
            var client = Models.Client.Create(addClient.Vpn, addClient.ClientId, null, addClient.Purposes.ToList());
            await _clientStore.Add(client, cancellationToken);
            return EventMeshPackageResult.SendResult(PackageResponseBuilder.Client(package.Header.Seq));
        }
    }
}
