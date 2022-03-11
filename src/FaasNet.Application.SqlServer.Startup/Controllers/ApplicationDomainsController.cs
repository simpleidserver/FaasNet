using FaasNet.Application.Core.ApplicationDomain.Commands;
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
    }
}
