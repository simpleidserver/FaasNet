using FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Queries
{
    public class GetApplicationDomainQuery : IRequest<ApplicationDomainResult>
    {
        public string ApplicationDomainId { get; set; }
    }
}
