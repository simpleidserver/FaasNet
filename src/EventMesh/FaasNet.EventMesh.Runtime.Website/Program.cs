using FaasNet.EventMesh.Runtime.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                    scope.ServiceProvider.SeedAMQPOptions();
                }

                if (Startup.GetBoolean(configuration, "Kafka.Enabled"))
                {
                    scope.ServiceProvider.SeedKafkaOptions();
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
    }
}
