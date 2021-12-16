using FaasNet.Gateway.Core.Functions.Commands;
using MediatR;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions
{
    public class FunctionService : IFunctionService
    {
        private readonly IMediator _mediator;

        public FunctionService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Publish(string name, string image, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PublishFunctionCommand { Image = image, Name = name }, cancellationToken);
        }

        public Task<JToken> Invoke(string id, JToken input, JObject configuration, CancellationToken cancellationToken)
        {
            return _mediator.Send(new InvokeFunctionCommand { Configuration = configuration, Id = id, Input = input }, cancellationToken);
        }

        public Task<bool> Unpublish(string id, CancellationToken cancellationToken)
        {
            return _mediator.Send(new UnpublishFunctionCommand { Id = id }, cancellationToken);
        }
    }
}
