using FaasNet.Runtime.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;

namespace FaasNet.Gateway.SqlServer.Startup.Infrastructure.Filters
{
    public class FaasNetExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var type = context.Exception.GetType();
            if (type == typeof(OpenAPIException))
            {
                var openApiException = context.Exception as OpenAPIException;
                context.Result = BuildError("BadOpenApiOperation", openApiException.Message, HttpStatusCode.BadRequest);
            }
        }

        private static IActionResult BuildError(string code, string message, HttpStatusCode httpStatus)
        {
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(new
                {
                    Code = code,
                    Message = message
                }),
                ContentType = "application/json",
                StatusCode = (int)httpStatus
            };
        }
    }
}
