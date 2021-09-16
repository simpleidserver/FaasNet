using MediatR;

namespace FaasNet.Gateway.Core.Functions.Commands
{
    public class UnpublishFunctionCommand : IRequest<bool>
    {
        public string Name { get; set; }
    }
}
