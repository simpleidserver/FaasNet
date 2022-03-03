using FaasNet.EventMesh.Runtime.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FaasNet.EventMesh.Runtime.Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EventMeshDBContext>();
                dbContext.Database.Migrate();
                var configuration = host.Services.GetRequiredService<IConfiguration>();
                if (Startup.GetBoolean(configuration, "RabbitMQ.Enabled"))
                {
                    scope.ServiceProvider.SeedAMQPOptions();
                }

                if (Startup.GetBoolean(configuration, "Kafka.Enabled"))
                {
                    scope.ServiceProvider.SeedKafkaOptions();
                }
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
