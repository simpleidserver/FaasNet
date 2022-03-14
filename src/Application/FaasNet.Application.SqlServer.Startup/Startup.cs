using FaasNet.Application.Core;
using FaasNet.EventStore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using System.Reflection;

namespace FaasNet.Application.SqlServer.Startup
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
            services.AddHttpClient();
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddSwaggerGen();
            services.AddApplication(evtStoreBuilderCallback: c => c.UseEF(opt =>opt.UseSqlServer(Configuration.GetConnectionString("Application"), o => o.MigrationsAssembly(migrationsAssembly)))
                .UseEventStoreDB(opt =>
                {
                    opt.ConnectionString = Configuration.GetConnectionString("EventStoreDB");
                })
                .UseEF(opt => opt.UseSqlServer(Configuration.GetConnectionString("Application"), o => o.MigrationsAssembly(migrationsAssembly))))
                .UseEF(opt => opt.UseSqlServer(Configuration.GetConnectionString("Application"), o => o.MigrationsAssembly(migrationsAssembly)));
            services.AddHostedService<ProjectionService>();
            services.AddControllers().AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
