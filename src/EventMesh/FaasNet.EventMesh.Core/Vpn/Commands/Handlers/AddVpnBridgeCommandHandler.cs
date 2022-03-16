using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Handlers
{
    public class AddVpnBridgeCommandHandler : IRequestHandler<AddVpnBridgeCommand, bool>
    {
        private readonly IVpnStore _vpnStore;

        public AddVpnBridgeCommandHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<bool> Handle(AddVpnBridgeCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            throw new System.NotImplementedException();
        }
    }
}
