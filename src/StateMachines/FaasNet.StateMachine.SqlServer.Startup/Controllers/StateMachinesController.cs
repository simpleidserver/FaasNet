using FaasNet.StateMachine.Core.StateMachines.Commands;
using FaasNet.StateMachine.Core.StateMachines.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.SqlServer.Startup.Controllers
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

        [HttpPost(".search")]
        public async Task<IActionResult> Search([FromBody] SearchStateMachinesQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartStateMachineCommand cmd, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddStateMachineCommand cmd, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(cmd, cancellationToken);
            return StatusCode((int)HttpStatusCode.Created, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateStateMachineCommand cmd, CancellationToken cancellationToken)
        {
            cmd.Id = id;
            var result = await _mediator.Send(cmd, cancellationToken);
            return StatusCode((int)HttpStatusCode.Created, result);
        }

        [HttpPut("{id}/info")]
        public async Task<IActionResult> UpdateInfo(string id, [FromBody] UpdateStateMachineInfoCommand cmd, CancellationToken cancellationToken)
        {
            cmd.Id = id;
            await _mediator.Send(cmd, cancellationToken);
            return new NoContentResult();
        }

        [HttpPost("empty")]
        public async Task<IActionResult> AddEmpty([FromBody] AddEmptyStateMachineCommand cmd, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(cmd, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = new JObject
                {
                    { "id", result }
                }.ToString()
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStateMachineDetailsQuery { Id = id }, cancellationToken);
            return new OkObjectResult(result);
        }

        #endregion
    }
}
