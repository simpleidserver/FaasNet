using FaasNet.Gateway.Core.EventMeshServer.Commands;
using FaasNet.Gateway.Core.EventMeshServer.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.SqlServer.Startup.Controllers
{
    [Route("eventmesh")]
    public class EventMeshServersController : Controller
    {
        private readonly IMediator _mediator;

        public EventMeshServersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddEventMeshServerCommand cmd, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllEventMeshServerQuery(), cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
