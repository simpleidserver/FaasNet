using FaasNet.Application.Core.ApplicationDomain.Commands.Results;
using FaasNet.Application.Core.Domains;
using FaasNet.EventStore;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Application.Core.ApplicationDomain.Commands.Handlers
{
    public class AddApplicationDomainCommandHandler : IRequestHandler<AddApplicationDomainCommand, AddApplicationDomainResult>
    {
        private readonly ICommitAggregateHelper _commitAggregateHelper;

        public AddApplicationDomainCommandHandler(ICommitAggregateHelper commitAggregateHelper)
        {
            _commitAggregateHelper = commitAggregateHelper;
        }

        public async Task<AddApplicationDomainResult> Handle(AddApplicationDomainCommand request, CancellationToken cancellationToken)
        {
            var applicationDomain = ApplicationDomainAggregate.Create(request.Name, request.Description, request.RootTopic);
            await _commitAggregateHelper.Commit(applicationDomain, cancellationToken);
            return new AddApplicationDomainResult
            {
                Id = applicationDomain.Id
            };
        }
    }
}
