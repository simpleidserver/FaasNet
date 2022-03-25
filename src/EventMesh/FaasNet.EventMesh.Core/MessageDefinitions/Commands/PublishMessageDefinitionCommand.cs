using FaasNet.EventMesh.Core.MessageDefinitions.Commands.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Commands
{
    public class PublishMessageDefinitionCommand : IRequest<PublishMessageDefinitionResult>
    {
        public string Id { get; set; }
    }
}
