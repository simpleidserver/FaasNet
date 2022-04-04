using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MassTransit;
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
        private readonly IBusControl _busControl;

        public UpdateApplicationDomainCommandHandler(IApplicationDomainRepository applicationDomainRepository, IBusControl busControl)
        {
            _applicationDomainRepository = applicationDomainRepository;
            _busControl = busControl;
        }

        public async Task<bool> Handle(UpdateApplicationDomainCommand request, CancellationToken cancellationToken)
        {
            var applicationDomain = await _applicationDomainRepository.Get(request.ApplicationDomainId, cancellationToken);
            if (applicationDomain == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_APPLICATIONDOMAIN, string.Format(Global.UnknownApplicationDomain, request.ApplicationDomainId));
            }

            var applications = request.Applications.Select(a => a.ToDomain()).ToList();
            applicationDomain.Update(applications);
            _applicationDomainRepository.Update(applicationDomain);
            await _applicationDomainRepository.SaveChanges(cancellationToken);
            foreach(var integrationEvent in applicationDomain.IntegrationEvents)
            {
                await _busControl.Publish(Convert.ChangeType(integrationEvent, integrationEvent.GetType()));
            }

            return true;
        }
    }
}
