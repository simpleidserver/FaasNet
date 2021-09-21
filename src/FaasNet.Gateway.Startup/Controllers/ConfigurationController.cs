using FaasNet.Gateway.Core.Configuration.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Startup.Controllers
{
    [Route("configuration")]
    public class ConfigurationController : Controller
    {
        private readonly IMediator _mediator;

        public ConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] ImportConfigurationCommand cmd, CancellationToken cancellatioNToken)
        {
            await _mediator.Send(cmd, cancellatioNToken);
            return new NoContentResult();
        }
    }
}
