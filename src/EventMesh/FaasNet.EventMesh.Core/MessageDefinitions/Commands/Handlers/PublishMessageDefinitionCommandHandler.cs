using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.ApplicationDomains;
using FaasNet.EventMesh.Core.MessageDefinitions.Commands.Results;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Commands.Handlers
{
    public class PublishMessageDefinitionCommandHandler : IRequestHandler<PublishMessageDefinitionCommand, PublishMessageDefinitionResult>
    {
        private readonly IMessageDefinitionRepository _messageDefinitionRepostory;

        public PublishMessageDefinitionCommandHandler(IMessageDefinitionRepository messageDefinitionRepository)
        {
            _messageDefinitionRepostory = messageDefinitionRepository;
        }

        public async Task<PublishMessageDefinitionResult> Handle(PublishMessageDefinitionCommand request, CancellationToken cancellationToken)
        {
            var messageDefinition = await _messageDefinitionRepostory.Get(request.Id, cancellationToken);
            if (messageDefinition == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_MESSAGE_DEFINITION, string.Format(Global.UnknownMessageDefinition, request.Id));
            }

            var newMessageDef = messageDefinition.Publish();
            _messageDefinitionRepostory.Add(newMessageDef);
            await _messageDefinitionRepostory.SaveChanges(cancellationToken);
            return new PublishMessageDefinitionResult { Id = newMessageDef.Id };
        }
    }
}
