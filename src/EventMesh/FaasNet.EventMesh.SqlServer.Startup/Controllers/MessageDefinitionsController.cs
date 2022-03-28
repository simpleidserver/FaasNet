using FaasNet.EventMesh.Core.MessageDefinitions.Commands;
using FaasNet.EventMesh.Core.MessageDefinitions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.SqlServer.Startup.Controllers
{
    [Route("messagedefs")]
    public class MessageDefinitionsController : Controller
    {
        private readonly IMediator _mediator;

        public MessageDefinitionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddMessageDef([FromBody] AddMessageDefinitionCommand cmd, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(cmd, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json"
            };
        }

        [HttpGet(".search/{id}/latest")]
        public async Task<IActionResult> GetAllLatestMessages(string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllLatestMessageDefQuery { ApplicationDomainId = id }, cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AddMessageDef(string id, [FromBody] UpdateMessageDefinitionCommand cmd, CancellationToken cancellationToken)
        {
            cmd.Id = id;
            await _mediator.Send(cmd, cancellationToken);
            return new NoContentResult();
        }

        [HttpGet("{id}/publish")]
        public async Task<IActionResult> PublishMessageDef(string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new PublishMessageDefinitionCommand { Id = id }, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json"
            };
        }
    }
}
