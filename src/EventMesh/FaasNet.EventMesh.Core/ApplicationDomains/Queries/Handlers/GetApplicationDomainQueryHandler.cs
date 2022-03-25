using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Queries.Handlers
{
    public class GetApplicationDomainQueryHandler : IRequestHandler<GetApplicationDomainQuery, ApplicationDomainResult>
    {
        private readonly IApplicationDomainRepository _applicationDomainRepository;

        public GetApplicationDomainQueryHandler(IApplicationDomainRepository applicationDomainRepository)
        {
            _applicationDomainRepository = applicationDomainRepository;
        }

        public async Task<ApplicationDomainResult> Handle(GetApplicationDomainQuery request, CancellationToken cancellationToken)
        {
            var applicationDomain = await _applicationDomainRepository.Get(request.ApplicationDomainId, cancellationToken);
            if (applicationDomain == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_APPLICATIONDOMAIN, string.Format(Global.UnknownApplicationDomain, request.ApplicationDomainId));
            }

            return ApplicationDomainResult.Build(applicationDomain);
        }
    }
}
