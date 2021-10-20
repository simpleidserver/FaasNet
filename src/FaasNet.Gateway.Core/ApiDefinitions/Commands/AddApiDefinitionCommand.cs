using MediatR;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands
{
    public class AddApiDefinitionCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
