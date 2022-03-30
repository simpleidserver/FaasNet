using FaasNet.EventMesh.Core.ApplicationDomains;
using FaasNet.EventMesh.Core.Consumers;
using FaasNet.EventMesh.Runtime;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using System.Reflection;

namespace FaasNet.EventMesh.SqlServer.Startup
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
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddSwaggerGen();
            services.AddEventMesh(opt =>
            {
                opt.AccountId = 678128;
                opt.LicenseKey = "gubXjBR4DMjsdqw3";
                opt.Host = "geolite.info";
            }, configureMassTransit: x => {
                x.AddConsumer<ApplicationDomainConsumer>();
                x.UsingRabbitMq((c, t) =>
                {
                    var connectionString = "amqp://guest:guest@127.0.0.1:5672/";
                    t.Host(connectionString);
                    t.ConfigureEndpoints(c);
                });
            }).UseEF(opt => opt.UseSqlServer(Configuration.GetConnectionString("EventMesh")));
            services.AddControllers().AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var busControl = app.ApplicationServices.GetService<IBusControl>();
            busControl.Start();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway V1");
            });
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
