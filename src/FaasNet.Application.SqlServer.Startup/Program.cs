using FaasNet.Application.EF;
using FaasNet.EventStore.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FaasNet.Application.SqlServer.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<EventStoreDBContext>())
                {
                    context.Database.Migrate();
                    context.SaveChanges();
                }

                using (var context = scope.ServiceProvider.GetService<ApplicationDBContext>())
                {
                    context.Database.Migrate();
                    context.SaveChanges();
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
