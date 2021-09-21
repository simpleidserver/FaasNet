using FaasNet.Gateway.Core.Functions.Commands;
using MediatR;
using System;
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

        public Task<bool> Publish(string name, string image, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PublishFunctionCommand { Image = image, Name = name }, cancellationToken);
        }

        public Task<bool> Unpublish(string name, CancellationToken cancellationToken)
        {
            return _mediator.Send(new UnpublishFunctionCommand { Name = name }, cancellationToken);
        }
    }
}
