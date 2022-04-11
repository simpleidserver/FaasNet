using FaasNet.Common;
using FaasNet.StateMachine.WorkerHost.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace FaasNet.StateMachine.WorkerHost
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
            var dbPath = Path.Combine(WebHostEnvironment.ContentRootPath, "StateMachineWorker.db");
            services.AddGrpc();
            services.AddStateMachineWorker()
                .UseEventStoreDB(opt =>
                {
                    opt.ConnectionString = Configuration["EventStoreDBConnectionString"];
                })
                .UseEF(opt =>
                {
                    opt.UseSqlite($"Data Source={dbPath}", s => s.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name));
                })
                .UseEventMesh(opt =>
                {
                    opt.Url = Configuration["EventMesh:Url"];
                    opt.ClientId = Configuration["EventMesh:ClientId"];
                    opt.Password = Configuration["EventMesh:Password"];
                    opt.Port = int.Parse(Configuration["EventMesh:Port"]);
                });
            services.AddHostedService<EventConsumerHostedService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<StateMachineInstanceService>();
            });
        }
    }
}
