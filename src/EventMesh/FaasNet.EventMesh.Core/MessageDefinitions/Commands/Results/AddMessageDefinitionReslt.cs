using MediatR;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Commands.Results
{
    public class AddMessageDefinitionReslt : IRequest<string>
    {
        public string Id { get; set; }
    }
}
