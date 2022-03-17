using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Queries
{
    public class GetVpnQuery : IRequest<VpnResult>
    {
        public string Vpn { get; set; }
    }
}
