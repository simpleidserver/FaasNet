using FaasNet.EventMesh.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            RegisterEventMeshService(services).AddRuntimeEF(opt => opt.UseSqlServer(Configuration.GetConnectionString("EventMesh"), optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly)));
            services.AddHostedService<RuntimeHostedService>();
            services.AddSingleton(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            if(GetBoolean(Configuration, $"{kafka}.Enabled"))
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
    }
}
