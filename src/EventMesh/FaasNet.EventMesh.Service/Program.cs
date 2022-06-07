using FaasNet.Common;
using FaasNet.EventMesh.Protocols;
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
                    var discoveredProtocolPlugins = pluginPaths.Select(p =>
                    {
                        if (ProtocolPluginEntryDiscovery.TryExtract(p, out IDiscoveredPlugin discoveredPlugin)) return discoveredPlugin;
                        return null;
                    }).Where(p => p != null);
                    var options = hostContext.Configuration.Get<EventMeshServerOptions>();
                    var serverBuilder = services.AddEventMeshServer(consensusNodeCallback: o => o.Port = options.Port)
                        .UseRocksDB(o => 
                        { 
                            o.SubPath = $"node{options.Port}"; 
                        });
                    foreach (var discoveredPlugin in discoveredProtocolPlugins) discoveredPlugin.Load(serverBuilder.Services);
                    services.AddHostedService<EventMeshServerWorker>();
                }).Build();
await host.RunAsync();