using FaasNet.Gateway.Core.OpenApi.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.SqlServer.Startup.Controllers
{
    [Route("openapi")]
    public class OpenApiController : Controller
    {
        private readonly IMediator _mediator;

        public OpenApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("operations")]
        public async Task<IActionResult> GetOperations([FromBody] GetOpenApiOperationsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPost("operations/{id}")]
        public async Task<IActionResult> GetOperation(string id, [FromBody] GetOpenApiOperationQuery query, CancellationToken cancellationToken)
        {
            query.OperationId = id;
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
