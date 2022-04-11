using FaasNet.Common;
using FaasNet.StateMachine.Core.StateMachines;
using FaasNet.StateMachine.SqlServer.Startup.Infrastructure;
using FaasNet.StateMachine.SqlServer.Startup.Infrastructure.Filters;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using System;
using System.Reflection;

namespace FaasNet.StateMachine.SqlServer.Startup
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
            services.AddStateMachine(configureMassTransit: x =>
            {
                x.AddConsumer<StateMachineConsumer>();
                x.UsingRabbitMq((c, t) =>
                {
                    var connectionString = Configuration["RabbitMQConnectionString"];
                    t.Host(connectionString);
                    t.ConfigureEndpoints(c);
                });
            }).UseStateMachineDefEF(opt =>
            {
                opt.UseLazyLoadingProxies();
                opt.UseSqlServer(Configuration.GetConnectionString("Runtime"), o => o.MigrationsAssembly(migrationsAssembly));
            }).UseStateMachineInstanceElasticSearchStore(opt =>
            {
                opt.Settings = new Nest.ConnectionSettings(new Uri(Configuration["ElasticSearchUrl"]));
            });
            services.AddControllers(opts =>
            {
                opts.InputFormatters.Add(new YamlInputFormatter());
                opts.OutputFormatters.Add(new YamlOutputFormatter());
                opts.Filters.Add(new FaasNetExceptionFilter());
                opts.RespectBrowserAcceptHeader = true;
            }).AddNewtonsoftJson(opts =>
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
