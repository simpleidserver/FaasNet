using MediatR;

namespace FaasNet.Function.Core.Functions.Commands
{
    public class PublishFunctionCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Version { get; set; }
        public string Command { get; set; }
    }
}
