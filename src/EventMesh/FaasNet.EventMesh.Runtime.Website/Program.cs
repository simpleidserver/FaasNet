using FaasNet.EventMesh.Runtime.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using System;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // CheckOpenTelemetry();
            // return;
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var eventMeshDbContext = scope.ServiceProvider.GetRequiredService<EventMeshDBContext>();
                var messageBrokerDbContext = scope.ServiceProvider.GetRequiredService<MessageBrokerDBContext>();
                eventMeshDbContext.Database.Migrate();
                messageBrokerDbContext.Database.Migrate();
                var configuration = host.Services.GetRequiredService<IConfiguration>();
                if (Startup.GetBoolean(configuration, "RabbitMQ.Enabled"))
                {
#pragma warning disable 4014
                    scope.ServiceProvider.SeedAMQPOptions();
#pragma warning restore 4014
                }

                if (Startup.GetBoolean(configuration, "Kafka.Enabled"))
                {
#pragma warning disable 4014
                    scope.ServiceProvider.SeedKafkaOptions();
#pragma warning restore 4014
                }

                if (!eventMeshDbContext.VpnLst.Any())
                {
                    eventMeshDbContext.VpnLst.Add(Models.Vpn.Create("default", "default"));
                }

                if (!eventMeshDbContext.ClientLst.Any())
                {
                    eventMeshDbContext.ClientLst.Add(Models.Client.Create("default", "stateMachineClientId", null, new System.Collections.Generic.List<Client.Messages.UserAgentPurpose>
                    {
                        Client.Messages.UserAgentPurpose.PUB,
                        Client.Messages.UserAgentPurpose.SUB
                    }));
                }

                eventMeshDbContext.SaveChanges();
                messageBrokerDbContext.SaveChanges();
            }
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void CheckOpenTelemetry()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddOpenTelemetry((opt) =>
                {
                    opt.IncludeFormattedMessage = true;
                    opt.IncludeScopes = true;
                    opt.AddConsoleExporter();
                });
            });

            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Hello from {name} {price}.", "tomato", 2.99);
            Console.ReadLine();
        }
    }
}
