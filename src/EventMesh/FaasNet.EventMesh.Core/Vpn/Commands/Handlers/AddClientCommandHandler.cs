using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Handlers
{
    public class AddClientCommandHandler : IRequestHandler<AddClientCommand, bool>
    {
        private readonly IVpnStore _vpnStore;

        public AddClientCommandHandler(IVpnStore vpnStore)
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

            List<UserAgentPurpose> purposes = null;
            if (request.Purposes != null && request.Purposes.Any())
            {
                purposes = request.Purposes.Select(p => new UserAgentPurpose(p)).ToList();
            }

            vpn.AddClient(request.ClientId, null, purposes);
            _vpnStore.Update(vpn);
            await _vpnStore.SaveChanges(cancellationToken);
            return true;
        }
    }
}
