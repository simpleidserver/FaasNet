using FaasNet.StateMachine.Core.AsyncApi.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.SqlServer.Startup.Controllers
{
    [Route("asyncapi")]
    public class AsyncApiController : Controller
    {
        private readonly IMediator _mediator;

        public AsyncApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("operations")]
        public async Task<IActionResult> GetOperations([FromBody] GetAsyncApiOperationsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPost("operations/{id}")]
        public async Task<IActionResult> GetOperation(string id, [FromBody] GetAsyncApiOperationQuery query, CancellationToken cancellationToken)
        {
            query.OperationId = id;
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
