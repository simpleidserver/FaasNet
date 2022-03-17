using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Handlers
{
    public class GetClientQueryHandler : IRequestHandler<GetClientQuery, ClientResult>
    {
        private readonly IVpnStore _vpnStore;

        public GetClientQueryHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<ClientResult> Handle(GetClientQuery request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            var client = vpn.GetClient(request.ClientId);
            if (client == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_CLIENT, string.Format(Global.UnknownClient, request.ClientId));
            }

            return ClientResult.Build(client);
        }
    }
}
