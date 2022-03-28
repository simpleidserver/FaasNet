using FaasNet.EventMesh.Core.Vpn.Commands;
using FaasNet.EventMesh.Core.Vpn.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.SqlServer.Startup.Controllers
{
    [Route("vpns")]
    public class VpnsController : Controller
    {
        private readonly IMediator _mediator;

        public VpnsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddVpnCommand cmd, CancellationToken cancellationToken)
        {
            await _mediator.Send(cmd, cancellationToken);
            return new StatusCodeResult((int)HttpStatusCode.Created);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllVpnQuery(), cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetVpnQuery { Vpn = name }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteVpnCommand { Vpn = name }, cancellationToken);
            return new NoContentResult();
        }

        [HttpPost("{name}/bridges")]
        public async Task<IActionResult> AddBridge(string name, [FromBody] AddVpnBridgeCommand cmd, CancellationToken cancellationToken)
        {
            cmd.Vpn = name;
            await _mediator.Send(cmd, cancellationToken);
            return new StatusCodeResult((int)HttpStatusCode.Created);
        }
    }
}