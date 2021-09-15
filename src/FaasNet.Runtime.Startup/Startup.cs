using FaasNet.Runtime.Startup.Factories;
using FaasNet.Runtime.Startup.Parameters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Startup
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Map("/configuration", (c) =>
            {
                c.Run(async (httpContext) =>
                {
                    var configuration = ConfigurationFactory.New(typeof(HelloConfiguration));
                    var json = JsonConvert.SerializeObject(configuration);
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(json);
                });
            });
            app.Use(async (context, next) =>
            {
                if (context.Request.Method != HttpMethods.Post)
                {
                    await next.Invoke();
                    return;
                }

                var functionHandler = new FunctionHandler();
                try
                {
                    var requestBody = await GetRequest(context.Request.Body);
                    var parameter = JsonConvert.DeserializeObject<FunctionParameter<HelloConfiguration>>(requestBody);
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
            });
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
