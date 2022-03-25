using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Commands.Handlers
{
    public class RemoveApplicationDomainCommandHandler : IRequestHandler<RemoveApplicationDomainCommand, bool>
    {
        private readonly IApplicationDomainRepository _applicationDomainRepository;

        public RemoveApplicationDomainCommandHandler(IApplicationDomainRepository applicationDomainRepository)
        {
            _applicationDomainRepository = applicationDomainRepository;
        }

        public async Task<bool> Handle(RemoveApplicationDomainCommand request, CancellationToken cancellationToken)
        {
            var applicationDomain = await _applicationDomainRepository.Get(request.ApplicationDomainId, cancellationToken);
            if (applicationDomain == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_APPLICATIONDOMAIN, string.Format(Global.UnknownApplicationDomain, request.ApplicationDomainId));
            }

            _applicationDomainRepository.Delete(applicationDomain);
            await _applicationDomainRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
