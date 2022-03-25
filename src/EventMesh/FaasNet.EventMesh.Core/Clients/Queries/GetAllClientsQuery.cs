using FaasNet.EventMesh.Core.Clients.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.Clients.Queries
{
    public class GetAllClientsQuery : IRequest<IEnumerable<ClientResult>>
    {
        public string Vpn { get; set; }
    }
}
