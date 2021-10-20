using MediatR;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands
{
    public class AddApiDefinitionOperationCommand : IRequest<bool>
    {
        public string ApiName { get; set; }
        public string OpName { get; set; }
        public string OpPath { get; set; }
    }
}
