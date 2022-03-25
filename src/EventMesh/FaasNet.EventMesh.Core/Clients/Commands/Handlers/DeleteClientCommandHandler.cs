using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Clients.Commands.Handlers
{
    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, bool>
    {
        private readonly IClientStore _clientStore;

        public DeleteClientCommandHandler(IVpnService vpnService, IClientStore clientStore)
        {
            _clientStore = clientStore;
        }

        public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _clientStore.GetByClientId(request.Vpn, request.ClientId, cancellationToken);
            if (client == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_CLIENT, string.Format(Global.UnknownClient, request.ClientId));
            }

            _clientStore.Remove(client);
            await _clientStore.SaveChanges(cancellationToken);
            return true;
        }
    }
}
