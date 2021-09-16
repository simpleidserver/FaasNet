using FaasNet.Gateway.Core.ApiDefinitions.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Startup.Middlewares
{
    public class RequestApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMediator _mediator;

        public RequestApiMiddleware(
            RequestDelegate next, 
            IMediator mediator)
        {
            _next = next;
            _mediator = mediator;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method != HttpMethods.Post)
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.Value.TrimStart('/');
            var request = await GetRequest(context);
            var result = await _mediator.Send(new InvokeApiDefinitionCommand
            {
                FullPath = path,
                Content = request
            }, CancellationToken.None);
            if (!result.MatchApi)
            {
                await _next(context);
                return;
            }

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result.Content.ToString());
        }

        public async Task<JObject> GetRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                var str = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                if (string.IsNullOrWhiteSpace(str))
                {
                    return null;
                }

                return JObject.Parse(str);
            }
        }
    }
}
