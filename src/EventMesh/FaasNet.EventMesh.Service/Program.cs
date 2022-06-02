using FaasNet.Common;
using FaasNet.EventMesh.Service;

using IHost host = Host.CreateDefaultBuilder(args)
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
                    var options = hostContext.Configuration.Get<EventMeshServerOptions>();
                    services.AddEventMeshServer(consensusNodeCallback: o => o.Port = options.Port)
                        .UseRocksDB(o => 
                        { 
                            o.SubPath = $"node{options.Port}"; 
                        });
                    services.AddHostedService<EventMeshServerWorker>();
                }).Build();
await host.RunAsync();