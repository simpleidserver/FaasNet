using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.ApplicationDomains;
using FaasNet.EventMesh.Core.MessageDefinitions.Commands.Results;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Commands.Handlers
{
    public class AddMessageDefinitionCommandHandler : IRequestHandler<AddMessageDefinitionCommand, AddMessageDefinitionReslt>
    {
        private readonly IApplicationDomainService _applicationDomainService;
        private readonly IMessageDefinitionRepository _messageDefinitionRepository;

        public AddMessageDefinitionCommandHandler(IApplicationDomainService applicationDomainService, IMessageDefinitionRepository messageDefinitionRepository)
        {
            _applicationDomainService = applicationDomainService;
            _messageDefinitionRepository = messageDefinitionRepository;
        }

        public async Task<AddMessageDefinitionReslt> Handle(AddMessageDefinitionCommand request, CancellationToken cancellationToken)
        {
            var vpn = await _applicationDomainService.Get(request.ApplicationDomainId, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_APPLICATIONDOMAIN, string.Format(Global.UnknownApplicationDomain, request.ApplicationDomainId));
            }

            var messageDef = MessageDefinition.Create(request.ApplicationDomainId, request.Name, request.Description, request.JsonSchema);
            _messageDefinitionRepository.Add(messageDef);
            await _messageDefinitionRepository.SaveChanges(cancellationToken);
            return new AddMessageDefinitionReslt { Id = messageDef.Id };
        }
    }
}
