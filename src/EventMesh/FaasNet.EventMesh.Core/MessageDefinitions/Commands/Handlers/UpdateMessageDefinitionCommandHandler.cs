using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Commands.Handlers
{
    public class UpdateMessageDefinitionCommandHandler : IRequestHandler<UpdateMessageDefinitionCommand, bool>
    {
        private readonly IMessageDefinitionRepository _messageDefinitionRepository;

        public UpdateMessageDefinitionCommandHandler(IMessageDefinitionRepository messageDefinitionRepository)
        {
            _messageDefinitionRepository = messageDefinitionRepository;
        }

        public async Task<bool> Handle(UpdateMessageDefinitionCommand request, CancellationToken cancellationToken)
        {
            var messageDef = await _messageDefinitionRepository.Get(request.Id, cancellationToken);
            if (messageDef == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_MESSAGE_DEFINITION, string.Format(Global.UnknownMessageDefinition, request.Id));
            }

            messageDef.Update(request.Description, request.JsonSchema);
            _messageDefinitionRepository.Update(messageDef);
            await _messageDefinitionRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
