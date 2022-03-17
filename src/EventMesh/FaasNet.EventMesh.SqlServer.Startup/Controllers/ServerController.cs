using FaasNet.EventMesh.Core.Server.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.SqlServer.Startup.Controllers
{
    [Route("server")]
    public class ServerController : Controller
    {
        private readonly IMediator _mediator;

        public ServerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("status")]
        public async Task<IActionResult> Status(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetServerStatusQuery(), cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
