using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Handlers
{
    public class UpdateApplicationDomainCommandHandler : IRequestHandler<UpdateApplicationDomainCommand, bool>
    {
        private readonly IVpnStore _vpnStore;

        public UpdateApplicationDomainCommandHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<bool> Handle(UpdateApplicationDomainCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            vpn.UpdateApplicationDomain(request.ApplicationDomainId, request.Applications?.Select(a => a.ToDomain()).ToList());
            _vpnStore.Update(vpn);
            await _vpnStore.SaveChanges(cancellationToken);
            return true;
        }
    }
}
