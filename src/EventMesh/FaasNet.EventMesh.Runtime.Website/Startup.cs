using FaasNet.EventMesh.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Reflection;

namespace FaasNet.EventMesh.Runtime.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            services.AddRazorPages();
            services.AddServerSideBlazor();
            RegisterEventMeshService(services)
                .AddRuntimeEF(opt =>
                {
                    opt.UseSqlServer(Configuration.GetConnectionString("EventMesh"), optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly));
                    opt.LogTo(msg => Debug.WriteLine((msg)));
                })
                .AddMessageBrokerEF(opt =>
                {
                    opt.UseSqlServer(Configuration.GetConnectionString("EventMesh"), optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly));
                    opt.LogTo(msg => Debug.WriteLine((msg)));
                });
            services.AddHostedService<RuntimeHostedService>();
            services.AddSingleton(Configuration);
            var serviceName = "EventMeshServer";
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: assemblyVersion, serviceInstanceId: Environment.MachineName);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
                loggingBuilder.AddOpenTelemetry((opt) =>
                {
                    opt.SetResourceBuilder(resourceBuilder);
                    opt.IncludeFormattedMessage = true;
                    opt.AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri("http://localhost:30073/v1/logs");
                        o.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
                });
            });
            InitMeterExporter(resourceBuilder);
        }

        #region Dependencies

        private IServiceCollection RegisterEventMeshService(IServiceCollection services)
        {
            const string rabbitmq = "RabbitMQ";
            const string kafka = "Kafka";
            var broker = services.AddRuntimeWebsite(opt =>
            {
                opt.Urn = "localhost";
                opt.Port = 4000;
            });
            if (GetBoolean(Configuration, "InMemory.Enabled"))
            {
                broker.AddInMemoryMessageBroker();
            }

            if (GetBoolean(Configuration, $"{rabbitmq}.Enabled"))
            {
                broker.AddAMQP(opt =>
                {
                    int? port = null;
                    string host = null, userName = null, password = null;
                    TryGetStr(Configuration, $"{rabbitmq}.HostName", out host);
                    TryGetInt(Configuration, $"{rabbitmq}.Port", out port);
                    TryGetStr(Configuration, $"{rabbitmq}.UserName", out userName);
                    TryGetStr(Configuration, $"{rabbitmq}.Password", out password);
                    if (!string.IsNullOrWhiteSpace(host) ||
                        port != null ||
                        !string.IsNullOrWhiteSpace(userName) ||
                        !string.IsNullOrWhiteSpace(password))
                    {
                        opt.ConnectionFactory = (o) =>
                        {
                            if (!string.IsNullOrWhiteSpace(host)) o.HostName = host;
                            if (port != null) o.Port = port.Value;
                            if (!string.IsNullOrWhiteSpace(userName)) o.UserName = userName;
                            if (!string.IsNullOrWhiteSpace(password)) o.UserName = password;
                        };
                    }
                });
            }

            if (GetBoolean(Configuration, $"{kafka}.Enabled"))
            {
                broker.AddKafka(opt =>
                {
                    string bootstrapServers = null;
                    if (TryGetStr(Configuration, $"{kafka}.BootstrapServers", out bootstrapServers))
                    {
                        opt.BootstrapServers = bootstrapServers;
                    }
                });
            }

            return services;
        }

        private static void InitMeterExporter(ResourceBuilder resourceBuilder)
        {
            var providerBuilder = Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddMeter(EventMeshMeter.Name)
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:30073/v1/metrics");
                    o.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
            providerBuilder.Build();
        }

        #endregion

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        #region Helper used to get configuration

        public static bool TryGetInt(IConfiguration configuration, string name, out int? result)
        {
            result = null;
            var str = configuration[name];
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (int.TryParse(str, out int r))
                {
                    result = r;
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetStr(IConfiguration configuration, string name, out string result)
        {
            result = null;
            var str = configuration[name];
            if (!string.IsNullOrWhiteSpace(str))
            {
                result = str;
                return true;
            }

            return false;
        }

        public static bool TryGetEnum<T>(IConfiguration configuration, string name, out T? result) where T : struct
        {
            result = null;
            var str = string.Empty;
            if (!TryGetStr(configuration, name, out str))
            {
                return false;
            }

            if(Enum.TryParse(str, true, out T r))
            {
                result = r;
                return true;
            }

            return false;
        }

        public static bool GetBoolean(IConfiguration configuration, string name)
        {
            var str = configuration[name];
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            if(bool.TryParse(str, out bool result))
            {
                return result;
            }

            return false;
        }

        #endregion
    }
}
