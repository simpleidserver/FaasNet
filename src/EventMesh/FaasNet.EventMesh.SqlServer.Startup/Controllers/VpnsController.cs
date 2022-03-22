using FaasNet.EventMesh.Core.Vpn.Commands;
using FaasNet.EventMesh.Core.Vpn.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
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

        [HttpGet("{name}/domains")]
        public async Task<IActionResult> GetApplicationDomains(string name, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllApplicationDomainsQuery { Vpn = name }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPost("{name}/domains")]
        public async Task<IActionResult> AddApplicationDomain(string name, [FromBody] AddApplicationDomainCommand cmd, CancellationToken cancellationToken)
        {
            cmd.Vpn = name;
            var result = await _mediator.Send(cmd, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json"
            };
        }

        [HttpGet("{name}/domains/{id}")]
        public async Task<IActionResult> GetApplicationDomain(string name, string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllApplicationDomainQuery { Vpn = name, ApplicationDomainId = id }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpDelete("{name}/domains/{id}")]
        public async Task<IActionResult> RemoveApplicationDomain(string name, string id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RemoveApplicationDomainCommand { Vpn = name, ApplicationDomainId = id }, cancellationToken);
            return new NoContentResult();
        }

        [HttpGet("{name}/domains/{id}/messages/latest")]
        public async Task<IActionResult> GetAllLatestMessages(string name, string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllLatestMessageDefQuery { ApplicationDomainId = id, Vpn = name }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPost("{name}/domains/{id}/messages")]
        public async Task<IActionResult> AddMessageDef(string name, string id, [FromBody] AddMessageVpnCommand cmd, CancellationToken cancellationToken)
        {
            cmd.Vpn = name;
            cmd.ApplicationDomainId = id;
            var result = await _mediator.Send(cmd, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json"
            };
        }

        [HttpPut("{name}/domains/{id}/messages/{messageId}")]
        public async Task<IActionResult> AddMessageDef(string name, string id, string messageId, [FromBody] UpdateMessageVpnCommand cmd, CancellationToken cancellationToken)
        {
            cmd.Vpn = name;
            cmd.ApplicationDomainId = id;
            cmd.MessageId = messageId;
            await _mediator.Send(cmd, cancellationToken);
            return new NoContentResult();
        }

        [HttpGet("{name}/domains/{id}/messages/{messageName}/publish")]
        public async Task<IActionResult> PublishMessageDef(string name, string id, string messageName, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new PublishMessageVpnCommand { ApplicationDomainId = id, Vpn = name, Name = messageName }, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json"
            };
        }

        [HttpGet("{name}/clients")]
        public async Task<IActionResult> GetClients(string name, CancellationToken cancellationToken)
        {
            var clients = await _mediator.Send(new GetAllClientsQuery { Vpn = name }, cancellationToken);
            return new OkObjectResult(clients);
        }

        [HttpPost("{name}/clients")]
        public async Task<IActionResult> AddClient(string name, [FromBody] AddClientCommand cmd, CancellationToken cancellationToken)
        {
            cmd.Vpn = name;
            await _mediator.Send(cmd, cancellationToken);
            return new StatusCodeResult((int)HttpStatusCode.Created);
        }

        [HttpDelete("{name}/clients/{clientId}")]
        public async Task<IActionResult> DeleteClient(string name, string clientId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteClientCommand { Vpn = name, ClientId = clientId }, cancellationToken);
            return new NoContentResult();
        }

        [HttpGet("{name}/clients/{clientId}")]
        public async Task<IActionResult> GetClient(string name, string clientId, CancellationToken cancellationToken)
        {
            var client = await _mediator.Send(new GetClientQuery { ClientId = clientId, Vpn = name }, cancellationToken);
            return new OkObjectResult(client);
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