using FaasNet.Common;
using FaasNet.StateMachine.Worker;
using FaasNet.StateMachine.WorkerHost.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System;
using System.IO;
using System.Reflection;

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
            var loggerFactory = new LoggerFactory();
            services.AddStateMachineWorker()
                .UseEventStoreDB(opt =>
                {
                    opt.ConnectionString = Configuration["EventStoreDBConnectionString"];
                })
                .UseEF(opt =>
                {
                    opt.UseLoggerFactory(loggerFactory);
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
            var serviceName = "StateMachineWorker";
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: assemblyVersion, serviceInstanceId: Environment.MachineName);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddOpenTelemetry((opt) =>
                {
                    opt.SetResourceBuilder(resourceBuilder);
                    opt.IncludeFormattedMessage = true;
                    opt.AddConsoleExporter();
                    opt.AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(Configuration["OpenTelemetry:Otlp:Logs"]);
                        o.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
                });
            });
            InitMeterExporter(resourceBuilder);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<StateMachineInstanceService>();
            });
        }

        private void InitMeterExporter(ResourceBuilder resourceBuilder)
        {
            var providerBuilder = Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddMeter(StateMachineWorkerOptions.DefaultName)
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(Configuration["OpenTelemetry:Otlp:Metrics"]);
                    o.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
            providerBuilder.Build();
        }
    }
}
