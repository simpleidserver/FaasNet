using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Handlers
{
    public class AddVpnCommandHandler : IRequestHandler<AddVpnCommand, bool>
    {
        private readonly IVpnStore _vpnStore;

        public AddVpnCommandHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<bool> Handle(AddVpnCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Name, cancellationToken);
            if (vpn != null)
            {
                throw new BusinessRuleException(ErrorCodes.VPN_ALREADY_EXISTS, Global.VpnAlreadyExists);
            }

            vpn = Runtime.Models.Vpn.Create(request.Name, request.Description);
            // TODO : Raise integration event & add a new client.
            _vpnStore.Add(vpn);
            await _vpnStore.SaveChanges(cancellationToken);
            return true;
        }
    }
}
