using MediatR;

namespace FaasNet.Gateway.Core.Functions.Commands
{
    public class PublishFunctionCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
