using FaasNet.Common;
using FaasNet.EventMesh.Protocols;
using FaasNet.EventMesh.Runtime;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.EventMesh.Service;
using FaasNet.EventMesh.Sink;

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
                    var serverBuilder = services.AddEventMeshServer(consensusNodeCallback: o => o.Port = options.Port)
                        .UseRocksDB(o =>
                        {
                            o.SubPath = $"node{options.Port}";
                        })
                        .UseSinkRocksDB();
                    var pluginStore = serverBuilder.ServiceProvider.GetRequiredService<IPluginStore>();
                    var activePlugins = pluginStore.GetActivePlugins();
                    LoadProtocolPlugins(serverBuilder.Services, activePlugins);
                    LoadSinkPlugins(serverBuilder.Services, activePlugins);
                }).Build();
await host.RunAsync();

static void LoadProtocolPlugins(IServiceCollection services, IEnumerable<string> activePlugins)
{
    var pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.ProtocolsPluginSubPath);
    IEnumerable<string> pluginPaths = new string[0];
    if (Directory.Exists(pluginsDirectory)) pluginPaths = Directory.EnumerateDirectories(pluginsDirectory);
    var discoveredProtocolPlugins = pluginPaths.Select(p =>
    {
        if (ProtocolPluginEntryDiscovery.TryExtract(p, activePlugins, out FaasNet.EventMesh.Protocols.IDiscoveredPlugin discoveredPlugin)) return discoveredPlugin;
        return null;
    }).Where(p => p != null);
    foreach (var discoveredPlugin in discoveredProtocolPlugins) discoveredPlugin.Load(services);
    services.AddHostedService<EventMeshServerWorker>();
}

static void LoadSinkPlugins(IServiceCollection services, IEnumerable<string> activePlugins)
{
    var pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.SinksPluginSubPath);
    IEnumerable<string> pluginPaths = new string[0];
    if (Directory.Exists(pluginsDirectory)) pluginPaths = Directory.EnumerateDirectories(pluginsDirectory);
    var discoveredSinkPlugins = pluginPaths.Select(p =>
    {
        if (SinkPluginEntryDiscovery.TryExtract(p, activePlugins, out FaasNet.EventMesh.Sink.IDiscoveredPlugin discoveredPlugin)) return discoveredPlugin;
        return null;
    }).Where(p => p != null);
    foreach (var discoveredPlugin in discoveredSinkPlugins) discoveredPlugin.Load(services);
    services.AddHostedService<EventMeshServerSinkWorker>();
}