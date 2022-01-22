using FaasNet.Runtime.Tests.Bus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Saunter;

namespace FaasNet.Runtime.Tests
{
    public class FakeStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.AddControllers(); 
            services.AddAsyncApiSchemaGeneration(options =>
            {
                // Specify example type(s) from assemblies to scan.
                options.AssemblyMarkerTypes = new[] { typeof(StreetlightMessageBus) };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway V1");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAsyncApiDocuments();
                endpoints.MapControllers();
            });
        }
    }
}
