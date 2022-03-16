using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Handlers
{
    public class GetAllVpnQueryHandler : IRequestHandler<GetAllVpnQuery, IEnumerable<VpnResult>>
    {
        private readonly IVpnStore _vpnStore;

        public GetAllVpnQueryHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<IEnumerable<VpnResult>> Handle(GetAllVpnQuery request, CancellationToken cancellationToken)
        {
            var result = await _vpnStore.GetAll(cancellationToken);
            return result.Select(r => VpnResult.Build(r));
        }
    }
}
