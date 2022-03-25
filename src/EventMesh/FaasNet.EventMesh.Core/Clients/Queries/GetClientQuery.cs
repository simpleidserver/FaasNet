using FaasNet.EventMesh.Core.Clients.Queries.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.Clients.Queries
{
    public class GetClientQuery : IRequest<ClientResult>
    {
        public string Id { get; set; }
    }
}