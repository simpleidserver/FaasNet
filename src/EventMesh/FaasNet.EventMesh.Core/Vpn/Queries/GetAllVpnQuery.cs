using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.Vpn.Queries
{
    public class GetAllVpnQuery : IRequest<IEnumerable<VpnResult>>
    {
    }
}
