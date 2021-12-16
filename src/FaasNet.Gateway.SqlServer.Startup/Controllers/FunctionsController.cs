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


        [HttpPost(".search")]
        public async Task<IActionResult> Search([FromBody] SearchFunctionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
