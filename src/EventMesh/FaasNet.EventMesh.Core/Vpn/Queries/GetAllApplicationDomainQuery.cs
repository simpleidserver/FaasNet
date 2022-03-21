using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Queries
{
    public class GetAllApplicationDomainQuery : IRequest<ApplicationDomainResult>
    {
        public string Vpn { get; set; }
        public string ApplicationDomainId { get; set; }
    }
}
