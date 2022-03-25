using FaasNet.EventMesh.Core.ApplicationDomains.Queries;
using FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.ApplicationDomains
{
    public class ApplicationDomainService : IApplicationDomainService
    {
        private readonly IMediator _mediator;

        public ApplicationDomainService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<ApplicationDomainResult> Get(string id, CancellationToken cancellationToken)
        {
            return _mediator.Send(new GetApplicationDomainQuery { ApplicationDomainId = id }, cancellationToken);
        }
    }
}
