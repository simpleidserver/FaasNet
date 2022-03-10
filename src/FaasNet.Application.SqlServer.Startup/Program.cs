using FaasNet.Application.Core.ApplicationDomain.Queries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FaasNet.Application.SqlServer.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var proj = new ApplicationDomainQueryProjection();
            proj.Project(new EventStore.ProjectionBuilder());
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
