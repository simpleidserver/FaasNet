using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Handlers
{
    public class RemoveApplicationDomainCommandHandler : IRequestHandler<RemoveApplicationDomainCommand, bool>
    {
        private readonly IVpnStore _vpnStore;

        public RemoveApplicationDomainCommandHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<bool> Handle(RemoveApplicationDomainCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            vpn.RemoveApplicationDomain(request.ApplicationDomainId);
            _vpnStore.Update(vpn);
            await _vpnStore.SaveChanges(cancellationToken);
            return true;
        }
    }
}
