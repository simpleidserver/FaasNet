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
        private readonly IVpnStore _vpnStore;
        private readonly IClientStore _clientStore;

        public AddClientMessageHandler(IVpnStore vpnStore, IClientStore clientStore)
        {
            _vpnStore = vpnStore;
            _clientStore = clientStore;
        }

        public Commands Command => Commands.ADD_CLIENT_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var addClient = package as AddClientRequest;
            var existingVpn = await _vpnStore.Get(addClient.Vpn, cancellationToken);
            if (existingVpn == null) return EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(package.Header.Command, package.Header.Seq, Errors.UNKNOWN_VPN));
            var existingClient = await _clientStore.Get(addClient.Vpn, addClient.ClientId, cancellationToken);
            if (existingClient != null) return EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(package.Header.Command, package.Header.Seq, Errors.CLIENT_ALREADY_EXISTS));
            var client = Models.Client.Create(addClient.Vpn, addClient.ClientId, null, addClient.Purposes.ToList());
            await _clientStore.Add(client, cancellationToken);
            return EventMeshPackageResult.SendResult(PackageResponseBuilder.Client(package.Header.Seq));
        }
    }
}
