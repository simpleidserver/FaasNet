using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.Vpn.Queries
{
    public class GetAllClientsQuery : IRequest<IEnumerable<ClientResult>>
    {
        public string Vpn { get; set; }
    }
}
