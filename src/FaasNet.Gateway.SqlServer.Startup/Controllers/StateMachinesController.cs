﻿using FaasNet.Gateway.Core.StateMachines.Commands;
using FaasNet.Gateway.Core.StateMachines.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStateMachineDetailsQuery { Id = id }, cancellationToken);
            return new OkObjectResult(result);
        }

        #endregion
    }
}
