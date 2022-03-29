using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Core.Clients.Commands.Results;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Clients.Commands.Handlers
{
    public class AddClientCommandHandler : IRequestHandler<AddClientCommand, AddClientResult>
    {
        private readonly IVpnService _vpnService;
        private readonly IClientStore _clientStore;

        public AddClientCommandHandler(IVpnService vpnService, IClientStore clientStore)
        {
            _vpnService = vpnService;
            _clientStore = clientStore;
        }

        public async Task<AddClientResult> Handle(AddClientCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnService.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            List<UserAgentPurpose> purposes = null;
            if (request.Purposes != null && request.Purposes.Any())
            {
                purposes = request.Purposes.Select(p => new UserAgentPurpose(p)).ToList();
            }

            var client = Runtime.Models.Client.Create(request.Vpn, request.ClientId, string.Empty, purposes);
            _clientStore.Add(client);
            await _clientStore.SaveChanges(cancellationToken);
            return new AddClientResult { Id = client.Id };
        }
    }
}
