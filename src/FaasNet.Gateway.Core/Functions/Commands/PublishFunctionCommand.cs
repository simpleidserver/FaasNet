using MediatR;

namespace FaasNet.Gateway.Core.Functions.Commands
{
    public class PublishFunctionCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public string Metadata { get; set; }
    }
}
