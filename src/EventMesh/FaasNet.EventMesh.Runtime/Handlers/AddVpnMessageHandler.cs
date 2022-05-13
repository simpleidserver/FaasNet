using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class AddVpnMessageHandler : IMessageHandler
    {
        private readonly IVpnStore _vpnStore;

        public AddVpnMessageHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public Commands Command => Commands.ADD_VPN_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var addVpn = package as AddVpnRequest;
            var vpn = Models.Vpn.Create(addVpn.Vpn, string.Empty);
            await _vpnStore.Add(vpn, cancellationToken);
            var result = PackageResponseBuilder.AddVpn(package.Header.Seq);
            return EventMeshPackageResult.SendResult(result);
        }
    }
}
