using FaasNet.Function;
using FaasNet.Function.Factories;
using Function.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Prometheus;
using System.IO;

namespace Function
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MetricReporter>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMetricServer();
            app.Map("/configuration", (c) =>
            {
                c.Run(async (httpContext) =>
                {
                    var configuration = ConfigurationFactory.New(typeof(HelloWorldConfiguration));
                    var json = JsonConvert.SerializeObject(configuration);
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(json);
                });
            });
            app.UseMiddleware<ResponseMetricMiddleware>();
            app.UseMiddleware<FunctionMiddleware>();
        }
    }
}
