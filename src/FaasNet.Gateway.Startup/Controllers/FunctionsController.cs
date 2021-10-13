using FaasNet.Gateway.Core.Functions.Commands;
using FaasNet.Gateway.Core.Functions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Startup.Controllers
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

        [HttpGet("{name}/monitoring")]
        public async Task<IActionResult> GetMonitoring(string name, CancellationToken cancellationToken)
        {
            var query = new GetFunctionMonitoringQuery { FuncName = name, Query = Request.QueryString.Value.TrimStart('?'), IsRange = false };
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{name}/monitoring/range")]
        public async Task<IActionResult> GetMonitoringRange(string name, CancellationToken cancellationToken)
        {
            var query = new GetFunctionMonitoringQuery { FuncName = name, Query = Request.QueryString.Value.TrimStart('?'), IsRange = true };
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{name}/details")]
        public async Task<IActionResult> GetDetails(string name, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetFunctionDetailsQuery { FuncName = name }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPost("{name}/invoke")]
        public async Task<IActionResult> Invoke(string name, [FromBody] InvokeFunctionCommand cmd, CancellationToken cancellationToken)
        {
            cmd.FuncName = name;
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name, CancellationToken cancellationToken)
        {
            var cmd = new GetFunctionQuery { FuncName = name };
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{name}/configuration")]
        public async Task<IActionResult> GetConfiguration(string name, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetFunctionConfigurationQuery { FuncName = name }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Unpublish(string name, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UnpublishFunctionCommand { Name = name }, cancellationToken);
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
