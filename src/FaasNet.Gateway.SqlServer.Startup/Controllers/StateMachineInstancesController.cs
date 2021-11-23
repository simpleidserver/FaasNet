using FaasNet.Gateway.Core.StateMachineInstances.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.SqlServer.Startup.Controllers
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

        #endregion
    }
}
