using FaasNet.Application.Core.Domains;
using FaasNet.Application.Core.Resources;
using FaasNet.Domain.Exceptions;
using FaasNet.EventStore;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Application.Core.ApplicationDomain.Commands.Handlers
{
    public class RemoveApplicationDomainCommandHandler : IRequestHandler<RemoveApplicationDomainCommand, bool>
    {
        private readonly ICommitAggregateHelper _commitAggregateHelper;

        public RemoveApplicationDomainCommandHandler(ICommitAggregateHelper commitAggregateHelper)
        {
            _commitAggregateHelper = commitAggregateHelper;
        }

        public async Task<bool> Handle(RemoveApplicationDomainCommand request, CancellationToken cancellationToken)
        {
            var result = await _commitAggregateHelper.Get<ApplicationDomainAggregate>(request.Id, cancellationToken);
            if (result == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_APPLICATION_DOMAIN, string.Format(Global.UnknownApplicationDomain, request.Id));
            }

            result.Remove();
            await _commitAggregateHelper.Commit(result, cancellationToken);
            return true;
        }
    }
}
