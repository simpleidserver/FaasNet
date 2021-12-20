using FaasNet.Gateway.Core.Functions.Commands;
using FaasNet.Gateway.Core.Functions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.SqlServer.Startup.Controllers
{
    [Route("functions")]
    public class FunctionsController : Controller
    {
        private readonly IMediator _mediator;

        public FunctionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] PublishFunctionCommand cmd, CancellationToken cancellationToken)
        {
            await _mediator.Send(cmd, cancellationToken);
            return new NoContentResult();
        }


        [HttpGet("{id}/monitoring")]
        public async Task<IActionResult> GetMonitoring(string id, CancellationToken cancellationToken)
        {
            var query = new GetFunctionMonitoringQuery { Id = id, Query = Request.QueryString.Value.TrimStart('?'), IsRange = false };
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}/monitoring/range")]
        public async Task<IActionResult> GetMonitoringRange(string id, CancellationToken cancellationToken)
        {
            var query = new GetFunctionMonitoringQuery { Id = id, Query = Request.QueryString.Value.TrimStart('?'), IsRange = true };
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetFunctionDetailsQuery { Id = id }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            var cmd = new GetFunctionQuery { Id = id };
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}/configuration")]
        public async Task<IActionResult> GetConfiguration(string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetFunctionConfigurationQuery { Id = id }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPost("{id}/invoke")]
        public async Task<IActionResult> Invoke(string id, [FromBody] InvokeFunctionCommand cmd, CancellationToken cancellationToken)
        {
            cmd.Id = id;
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Unpublish(string id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UnpublishFunctionCommand { Id = id }, cancellationToken);
            return new NoContentResult();
        }

        [HttpPost(".search")]
        public async Task<IActionResult> Search([FromBody] SearchFunctionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
