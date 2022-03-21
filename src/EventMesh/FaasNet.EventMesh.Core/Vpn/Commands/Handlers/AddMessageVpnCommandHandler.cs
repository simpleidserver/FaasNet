using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn.Commands.Results;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Handlers
{
    public class AddMessageVpnCommandHandler : IRequestHandler<AddMessageVpnCommand, AddMessageVpnResult>
    {
        private readonly IVpnStore _vpnStore;

        public AddMessageVpnCommandHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<AddMessageVpnResult> Handle(AddMessageVpnCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            var messageDef = MessageDefinition.Create(request.Name, request.Description, request.JsonSchema);
            vpn.AddMessage(request.ApplicationDomainId, messageDef);
            return new AddMessageVpnResult { Id = messageDef.Id };
        }
    }
}
