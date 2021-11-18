using FaasNet.Gateway.Core.StateMachines.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.SqlServer.Startup.Controllers
{
    [Route("statemachines")]
    public class StateMachinesController : Controller
    {
        private readonly IMediator _mediator;

        public StateMachinesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Actions

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartStateMachineCommand cmd, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        #endregion
    }
}
