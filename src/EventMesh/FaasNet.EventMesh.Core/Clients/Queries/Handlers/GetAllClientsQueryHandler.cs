using FaasNet.EventMesh.Core.Clients.Queries.Results;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Clients.Queries.Handlers
{
    public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, IEnumerable<ClientResult>>
    {
        private readonly IClientStore _clientStore;

        public GetAllClientsQueryHandler(IClientStore clientStore)
        {
            _clientStore = clientStore;
        }

        public async Task<IEnumerable<ClientResult>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = await _clientStore.GetAllByVpn(request.Vpn, cancellationToken);
            return clients.Select(c => ClientResult.Build(c));
        }
    }
}
