using FaasNet.Gateway.Core.ApiDefinitions.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions
{
    public class ApiDefinitionService : IApiDefinitionService
    {
        private readonly IMediator _mediator;

        public ApiDefinitionService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<bool> Replace(UpdateApiDefinitionCommand cmd, CancellationToken cancellationToken)
        {
            return _mediator.Send(cmd, cancellationToken);
        }
    }
}
