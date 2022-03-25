using MediatR;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Commands
{
    public class UpdateMessageDefinitionCommand : IRequest<bool>
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string JsonSchema { get; set; }
    }
}
