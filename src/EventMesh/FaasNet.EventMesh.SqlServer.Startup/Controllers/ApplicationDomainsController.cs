using FaasNet.EventMesh.Core.ApplicationDomains.Commands;
using FaasNet.EventMesh.Core.ApplicationDomains.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.SqlServer.Startup.Controllers
{
    [Route("domains")]
    public class ApplicationDomainsController : Controller
    {
        private readonly IMediator _mediator;

        public ApplicationDomainsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddApplicationDomain([FromBody] AddApplicationDomainCommand cmd, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(cmd, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json"
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationDomain(string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetApplicationDomainQuery { ApplicationDomainId = id }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplicationDomain(string id, [FromBody] UpdateApplicationDomainCommand cmd, CancellationToken cancellationToken)
        {
            cmd.ApplicationDomainId = id;
            await _mediator.Send(cmd, cancellationToken);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveApplicationDomain(string id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RemoveApplicationDomainCommand { ApplicationDomainId = id }, cancellationToken);
            return new NoContentResult();
        }

        [HttpGet(".search/{vpn}")]
        public async Task<IActionResult> SearchApplicationDomains(string vpn, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllApplicationDomainsQuery { Vpn = vpn }, cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
