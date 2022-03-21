using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Handlers
{
    public class UpdateMessageVpnCommandHandler : IRequestHandler<UpdateMessageVpnCommand, bool>
    {
        private readonly IVpnStore _vpnStore;

        public UpdateMessageVpnCommandHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<bool> Handle(UpdateMessageVpnCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            vpn.UpdateMessage(request.ApplicationDomainId, request.MessageId, request.Description, request.JsonSchema);
            _vpnStore.Update(vpn);
            await _vpnStore.SaveChanges(cancellationToken);
            return true;
        }
    }
}
