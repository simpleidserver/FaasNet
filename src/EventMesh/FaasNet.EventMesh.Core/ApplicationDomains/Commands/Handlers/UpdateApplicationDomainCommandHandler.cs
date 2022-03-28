using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Commands.Handlers
{
    public class UpdateApplicationDomainCommandHandler : IRequestHandler<UpdateApplicationDomainCommand, bool>
    {
        private readonly IApplicationDomainRepository _applicationDomainRepository;

        public UpdateApplicationDomainCommandHandler(IApplicationDomainRepository applicationDomainRepository)
        {
            _applicationDomainRepository = applicationDomainRepository;
        }

        public async Task<bool> Handle(UpdateApplicationDomainCommand request, CancellationToken cancellationToken)
        {
            var applicationDomain = await _applicationDomainRepository.Get(request.ApplicationDomainId, cancellationToken);
            if (applicationDomain == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_APPLICATIONDOMAIN, string.Format(Global.UnknownApplicationDomain, request.ApplicationDomainId));
            }

            applicationDomain.Applications = request.Applications.Select(a => a.ToDomain()).ToList();
            applicationDomain.UpdateDateTime = DateTime.UtcNow;
            _applicationDomainRepository.Update(applicationDomain);
            await _applicationDomainRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
