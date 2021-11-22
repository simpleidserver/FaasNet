using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace FaasNet.Gateway.Startup
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var prometheusFilePath = Configuration.GetValue<string>("prometheusFilePath");
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddGateway(opt => opt.PrometheusFilePath = prometheusFilePath);
            services.AddSwaggerGen();
            services.AddControllers().AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("Docker"))
            {
                var prometheusFilePath = Configuration.GetValue<string>("prometheusFilePath");
                if (!string.IsNullOrWhiteSpace(prometheusFilePath) && !File.Exists(prometheusFilePath))
                {
                    var sourceFileName = Path.Combine(WebHostEnvironment.ContentRootPath, "targets.json");
                    File.Copy(sourceFileName, prometheusFilePath);
                }
            }

            app.UseCors("AllowAll");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway V1");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
