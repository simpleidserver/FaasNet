using FaasNet.Function.Parameters;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FaasNet.Function.GetSql.Middlewares
{
    public class FunctionMiddleware
    {
        private readonly RequestDelegate _request;

        public FunctionMiddleware(RequestDelegate request)
        {
            _request = request;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method != HttpMethods.Post)
            {
                await _request.Invoke(context);
                return;
            }

            var functionHandler = new FunctionHandler();
            try
            {
                var requestBody = await GetRequest(context.Request.Body);
                var parameter = JsonConvert.DeserializeObject<FunctionParameter<GetSqlConfiguration>>(requestBody);
                var result = await functionHandler.Handle(parameter);
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result.ToString());
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.Message);
            }
        }

        private Task<string> GetRequest(Stream inputBody)
        {
            using (var reader = new StreamReader(inputBody))
            {
                return reader.ReadToEndAsync();
            }
        }
    }
}
