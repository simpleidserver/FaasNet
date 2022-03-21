using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn.Commands.Results;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Handlers
{
    public class PublishMessageVpnCommandHandler : IRequestHandler<PublishMessageVpnCommand, PublishVpnResult>
    {
        private readonly IVpnStore _vpnStore;

        public PublishMessageVpnCommandHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<PublishVpnResult> Handle(PublishMessageVpnCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            var id = vpn.PublishMessage(request.ApplicationDomainId, request.Name);
            _vpnStore.Update(vpn);
            await _vpnStore.SaveChanges(cancellationToken);
            return new PublishVpnResult { Id = id };
        }
    }
}
