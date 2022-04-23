using Elasticsearch.Net;
using FaasNet.EventMesh.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
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
                .AddRuntimeEF(opt => opt.UseSqlServer(Configuration.GetConnectionString("EventMesh"), optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly)))
                .AddMessageBrokerEF(opt => opt.UseSqlServer(Configuration.GetConnectionString("EventMesh"), optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly)));
            services.AddHostedService<RuntimeHostedService>();
            services.AddSingleton(Configuration);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                var settings = new ConnectionConfiguration();
                var autoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7;
                var batchAction = ElasticOpType.Create;
                if (TryGetEnum(Configuration, "Serilog:autoRegisterTemplateVersion", out AutoRegisterTemplateVersion? r))
                {
                    autoRegisterTemplateVersion = r.Value;
                }

                if (TryGetEnum(Configuration, "Serilog:batchAction", out ElasticOpType? op))
                {
                    batchAction = op.Value;
                }

                var logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Error)
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Configuration["Serilog:nodeUris"]))
                    {
                        ModifyConnectionSettings = x =>
                        {
                            x.BasicAuthentication(Configuration["Serilog:user"], Configuration["Serilog:password"]);
                            if (GetBoolean(Configuration, "Serilog:ignoreHttps"))
                            {
                                x.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);
                                x.ServerCertificateValidationCallback(CertificateValidations.AllowAll);
                            }
                            return x;
                        },
                        IndexFormat = Configuration["Serilog:indexFormat"],
                        AutoRegisterTemplateVersion = autoRegisterTemplateVersion,
                        TypeName = null,
                        BatchAction = batchAction,
                    })
                    .WriteTo.Console().CreateLogger();
                loggingBuilder.AddSerilog(logger);
            });
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
