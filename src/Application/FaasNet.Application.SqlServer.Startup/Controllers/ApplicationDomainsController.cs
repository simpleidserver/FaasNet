using FaasNet.Application.Core.ApplicationDomain.Commands;
using FaasNet.Application.Core.ApplicationDomain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Application.SqlServer.Startup.Controllers
{
    [Route("applicationdomains")]
    public class ApplicationDomainsController : Controller
    {
        private readonly IMediator _mediator;

        public ApplicationDomainsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddApplicationDomainCommand cmd, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(cmd, cancellationToken);
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(string id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RemoveApplicationDomainCommand { Id = id }, cancellationToken);
            return new NoContentResult();
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllApplicationDomainQuery(), cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
