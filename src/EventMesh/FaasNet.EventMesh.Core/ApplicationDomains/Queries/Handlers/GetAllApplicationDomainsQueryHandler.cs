using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Queries.Handlers
{
    public class GetAllApplicationDomainsQueryHandler : IRequestHandler<GetAllApplicationDomainsQuery, IEnumerable<ApplicationDomainResult>>
    {
        private readonly IApplicationDomainRepository _applicationDomainRepository;

        public GetAllApplicationDomainsQueryHandler(IApplicationDomainRepository applicationDomainRepository)
        {
            _applicationDomainRepository = applicationDomainRepository;
        }

        public async Task<IEnumerable<ApplicationDomainResult>> Handle(GetAllApplicationDomainsQuery request, CancellationToken cancellationToken)
        {
            var result = await _applicationDomainRepository.GetAll(request.Vpn, cancellationToken);
            return result.Select(c => ApplicationDomainResult.Build(c));
        }
    }
}
