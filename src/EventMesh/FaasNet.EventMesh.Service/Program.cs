using FaasNet.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FaasNet.EventMesh.Service
{
    internal class Program
    {
        public static void Main(string[] args) { }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService(o =>
                {
                    o.ServiceName = "EventMesh Service";
                })
                .ConfigureAppConfiguration(c =>
                {
                    c.AddJsonFile("appsettings.json");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var options = hostContext.Configuration.GetValue<EventMeshServerOptions>("eventmesh");
                    services.AddEventMeshServer(consensusNodeCallback: o => o.Port = options.Port)
                        .UseRocksDB(o => { o.SubPath = $"node{options.Port}"; });
                    services.AddHostedService<EventMeshServerWorker>();
                });
    }
}
