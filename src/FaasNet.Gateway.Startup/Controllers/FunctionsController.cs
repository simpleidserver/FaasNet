using FaasNet.Gateway.Core.Functions.Commands;
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


        [HttpDelete("{name}")]
        public async Task<IActionResult> Unpublish(string name, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UnpublishFunctionCommand { Name = name }, cancellationToken);
            return new NoContentResult();
        }
    }
}
