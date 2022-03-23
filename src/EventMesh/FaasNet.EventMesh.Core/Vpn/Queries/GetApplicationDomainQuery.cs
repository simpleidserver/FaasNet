using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Queries
{
    public class GetApplicationDomainQuery : IRequest<ApplicationDomainResult>
    {
        public string Vpn { get; set; }
        public string ApplicationDomainId { get; set; }
    }
}
