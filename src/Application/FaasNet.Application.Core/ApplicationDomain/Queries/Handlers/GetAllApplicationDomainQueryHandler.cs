using FaasNet.Application.Core.ApplicationDomain.Queries.Results;
using FaasNet.Application.Core.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Application.Core.ApplicationDomain.Queries.Handlers
{
    public class GetAllApplicationDomainQueryHandler : IRequestHandler<GetAllApplicationDomainQuery, IEnumerable<ApplicationDomainResult>>
    {
        private readonly IApplicationDomainQueryRepository _applicationDomainQueryRepository;

        public GetAllApplicationDomainQueryHandler(IApplicationDomainQueryRepository applicationDomainQueryRepository)
        {
            _applicationDomainQueryRepository = applicationDomainQueryRepository;
        }

        public Task<IEnumerable<ApplicationDomainResult>> Handle(GetAllApplicationDomainQuery request, CancellationToken cancellationToken)
        {
            return _applicationDomainQueryRepository.GetAll(cancellationToken);
        }
    }
}
