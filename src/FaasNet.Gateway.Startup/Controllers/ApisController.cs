using FaasNet.Gateway.Core.ApiDefinitions.Commands;
using FaasNet.Gateway.Core.ApiDefinitions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Startup.Controllers
{
    [Route("apis")]
    public class ApisController : Controller
    {
        private readonly IMediator _mediator;

        public ApisController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddApiDefinitionCommand cmd, CancellationToken cancellationToken)
        {
            await _mediator.Send(cmd, cancellationToken);
            return new NoContentResult();
        }

        [HttpPost("{name}/operations")]
        public async Task<IActionResult> AddOperation(string name, [FromBody] AddApiDefinitionOperationCommand cmd, CancellationToken cancellationToken)
        {
            cmd.ApiName = name;
            await _mediator.Send(cmd, cancellationToken);
            return new NoContentResult();
        }

        [HttpPut("{name}/operations/{opName}/ui")]
        public async Task<IActionResult> UpdateOperationUI(string name, string opName, [FromBody] UpdateApiDefinitionOperationUICommand cmd, CancellationToken cancellationToken)
        {
            cmd.FuncName = name;
            cmd.OperationName = opName;
            await _mediator.Send(cmd, cancellationToken);
            return new NoContentResult();
        }

        [HttpPost(".search")]
        public async Task<IActionResult> Search([FromBody] SearchApiDefinitionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetApiDefinitionQuery { Name = name }, cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
