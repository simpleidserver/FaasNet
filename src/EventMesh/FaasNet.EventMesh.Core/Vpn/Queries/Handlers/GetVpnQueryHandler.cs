using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Handlers
{
    public class GetVpnQueryHandler : IRequestHandler<GetVpnQuery, VpnResult>
    {
        private readonly IVpnStore _vpnStore;

        public GetVpnQueryHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<VpnResult> Handle(GetVpnQuery request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            return VpnResult.Build(vpn);
        }
    }
}
