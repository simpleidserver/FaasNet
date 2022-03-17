using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Handlers
{
    public class DeleteClientCommandHandler : IRequestHandler<AddClientCommand, bool>
    {
        private readonly IVpnStore _vpnStore;

        public DeleteClientCommandHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<bool> Handle(AddClientCommand request, CancellationToken cancellationToken)
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

            vpn.Clients.Remove(client);
            _vpnStore.Update(vpn);
            await _vpnStore.SaveChanges(cancellationToken);
            return true;
        }
    }
}
