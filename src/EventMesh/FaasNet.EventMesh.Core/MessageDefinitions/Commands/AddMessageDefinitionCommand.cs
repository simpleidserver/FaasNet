using FaasNet.EventMesh.Core.MessageDefinitions.Commands.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Commands
{
    public class AddMessageDefinitionCommand : IRequest<AddMessageDefinitionReslt>
    {
        public string ApplicationDomainId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string JsonSchema { get; set; }
    }
}
