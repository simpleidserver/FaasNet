using FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Queries
{
    public class GetAllApplicationDomainsQuery : IRequest<IEnumerable<ApplicationDomainResult>>
    {
        public string Vpn { get; set; }
    }
}
