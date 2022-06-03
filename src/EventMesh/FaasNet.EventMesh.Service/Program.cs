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
                    var pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
                    IEnumerable<string> pluginPaths = new string[0];
                    if(Directory.Exists(pluginsDirectory)) pluginPaths = Directory.EnumerateDirectories(pluginsDirectory);
                    // https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
                    /*
                    pluginPaths.SelectMany(p =>
                    {

                    });
                    */
                    var options = hostContext.Configuration.Get<EventMeshServerOptions>();
                    services.AddEventMeshServer(consensusNodeCallback: o => o.Port = options.Port)
                        .UseRocksDB(o => 
                        { 
                            o.SubPath = $"node{options.Port}"; 
                        });
                    services.AddHostedService<EventMeshServerWorker>();
                }).Build();
await host.RunAsync();