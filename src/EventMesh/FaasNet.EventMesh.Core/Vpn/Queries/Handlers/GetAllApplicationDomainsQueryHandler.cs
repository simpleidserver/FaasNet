using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Handlers
{
    public class GetAllApplicationDomainsQueryHandler : IRequestHandler<GetAllApplicationDomainsQuery, IEnumerable<ApplicationDomainResult>>
    {
        private readonly IVpnStore _vpnStore;

        public GetAllApplicationDomainsQueryHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<IEnumerable<ApplicationDomainResult>> Handle(GetAllApplicationDomainsQuery request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            return vpn.ApplicationDomains.Select(c => ApplicationDomainResult.Build(c));
        }
    }
}
