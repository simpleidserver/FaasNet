using FaasNet.Common;
using FaasNet.EventMesh.Service;
using FaasNet.Plugin;

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
                    var discoveredPlugins = pluginPaths.Select(p =>
                    {
                        if (PluginEntryDiscovery.TryExtract(p, out IDiscoveredPlugin discoveredPlugin)) return discoveredPlugin;
                        return null;
                    }).Where(p => p != null);
                    var options = hostContext.Configuration.Get<EventMeshServerOptions>();
                    var serverBuilder = services.AddEventMeshServer(consensusNodeCallback: o => o.Port = options.Port)
                        .UseRocksDB(o => 
                        { 
                            o.SubPath = $"node{options.Port}"; 
                        });
                    foreach (var discoveredPlugin in discoveredPlugins) discoveredPlugin.Load(serverBuilder.Services);
                    services.AddHostedService<EventMeshServerWorker>();
                }).Build();
await host.RunAsync();