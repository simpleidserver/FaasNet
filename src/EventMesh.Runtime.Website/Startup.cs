using EventMesh.Runtime;
using EventMesh.Runtime.EF;
using EventMesh.Runtime.MessageBroker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace EventMesh.Runtime.Website
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
            var path = Path.Combine(Environment.CurrentDirectory, "Runtime.db");
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHostedService<RuntimeHostedService>();
            RegisterEventMeshService(services).AddRuntimeEF(opt => opt.UseSqlite($"Data Source={path}", optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly)));
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

            Migrate(app.ApplicationServices);
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
                broker.AddInMemoryMessageBroker(new ConcurrentBag<InMemoryTopic>());
            }

            if (GetBoolean(Configuration, $"{rabbitmq}.Enabled"))
            {
                broker.AddAMQP(opt =>
                {
                    int? port = null;
                    string host = null, userName = null, password = null;
                    if(TryGetStr(Configuration, $"{rabbitmq}.HostName", out host) || 
                        TryGetInt(Configuration, $"{rabbitmq}.Port", out port) ||
                        TryGetStr(Configuration, $"{rabbitmq}.UserName", out userName) ||
                        TryGetStr(Configuration, $"{rabbitmq}.Password", out password))
                    {
                        opt.ConnectionFactory = (o) =>
                        {
                            if (!string.IsNullOrWhiteSpace(host)) o.HostName = host;
                            if (port != null) o.Port = port.Value;
                            if (!string.IsNullOrWhiteSpace(userName)) o.UserName = userName;
                            if (!string.IsNullOrWhiteSpace(password)) o.UserName = userName;
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

        private static void Migrate(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EventMeshDBContext>();
                dbContext.Database.Migrate();
                scope.ServiceProvider.SeedAMQPOptions();
                scope.ServiceProvider.SeedKafkaOptions();
            }
        }

        private static bool TryGetInt(IConfiguration configuration, string name, out int? result)
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

        private static bool TryGetStr(IConfiguration configuration, string name, out string result)
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

        private static bool GetBoolean(IConfiguration configuration, string name)
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
