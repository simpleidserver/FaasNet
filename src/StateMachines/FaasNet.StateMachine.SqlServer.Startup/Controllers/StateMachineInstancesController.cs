using FaasNet.StateMachine.Core.StateMachineInstances.Commands;
using FaasNet.StateMachine.Core.StateMachineInstances.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.SqlServer.Startup.Controllers
{
    [Route("statemachineinstances")]
    public class StateMachineInstancesController : Controller
    {
        private readonly IMediator _mediator;

        public StateMachineInstancesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Actions

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstance(string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStateMachineInstanceQuery { Id = id }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}/reactivate")]
        public async Task<IActionResult> Reactivate(string id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ReactivateStateMachineInstanceCommand { Id = id }, cancellationToken);
            return new NoContentResult();
        }


        [HttpPost(".search")]
        public async Task<IActionResult> Search([FromBody] SearchStateMachineInstanceQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }

        #endregion
    }
}
