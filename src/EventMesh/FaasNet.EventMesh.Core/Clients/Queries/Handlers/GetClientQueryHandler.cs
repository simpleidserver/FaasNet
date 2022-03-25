using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Clients.Queries.Results;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Clients.Queries.Handlers
{
    public class GetClientQueryHandler : IRequestHandler<GetClientQuery, ClientResult>
    {
        private readonly IClientStore _clientStore;

        public GetClientQueryHandler(IClientStore clientStore)
        {
            _clientStore = clientStore;
        }

        public async Task<ClientResult> Handle(GetClientQuery request, CancellationToken cancellationToken)
        {
            var client = await _clientStore.GetById(request.Id, cancellationToken);
            if (client == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_CLIENT, string.Format(Global.UnknownClient, request.Id));
            }

            return ClientResult.Build(client);
        }
    }
}
