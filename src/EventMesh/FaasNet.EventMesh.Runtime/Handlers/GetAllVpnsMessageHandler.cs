using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class GetAllVpnsMessageHandler : IMessageHandler
    {
        private readonly IVpnStore _vpnStore;

        public GetAllVpnsMessageHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public Commands Command => Commands.GET_ALL_VPNS_REQUEST;

        public async Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var vpns = await _vpnStore.GetAll(cancellationToken);
            var result = PackageResponseBuilder.GetAllVpns(package.Header.Seq, vpns.Select(v => v.Name));
            return EventMeshPackageResult.SendResult(result);
        }
    }
}
