using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Queries
{
    public class GetClientQuery : IRequest<ClientResult>
    {
        public string Vpn { get; set; }
        public string ClientId { get; set; }
    }
}