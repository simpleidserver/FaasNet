using FaasNet.EventMesh.Core.Clients.Commands;
using FaasNet.EventMesh.Core.Clients.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.SqlServer.Startup.Controllers
{
    [Route("clients")]
    public class ClientsController : Controller
    {
        private readonly IMediator _mediator;

        public ClientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] AddClientCommand cmd, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(cmd, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json"
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(string id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteClientCommand { Id = id }, cancellationToken);
            return new NoContentResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(string id, CancellationToken cancellationToken)
        {
            var client = await _mediator.Send(new GetClientQuery { Id = id}, cancellationToken);
            return new OkObjectResult(client);
        }

        [HttpGet(".search/{vpn}")]
        public async Task<IActionResult> GetClients(string name, CancellationToken cancellationToken)
        {
            var clients = await _mediator.Send(new GetAllClientsQuery { Vpn = name }, cancellationToken);
            return new OkObjectResult(clients);
        }
    }
}
