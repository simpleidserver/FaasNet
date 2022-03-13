using MediatR;

namespace FaasNet.Function.Core.Functions.Commands
{
    public class UnpublishFunctionCommand : IRequest<bool>
    {
        public string Id { get; set; }
    }
}
