using FaasNet.Common;
using FaasNet.EventMesh.Plugin;
using FaasNet.EventMesh.Protocols;
using FaasNet.EventMesh.Runtime;
using FaasNet.EventMesh.Service;
using FaasNet.EventMesh.Sink;
using FaasNet.RaftConsensus.Core.Stores;

using IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(o =>
                {
                    o.ServiceName = "EventMesh Service";
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json");
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    if(!string.IsNullOrWhiteSpace(environment)) config.AddJsonFile($"appsettings.{environment}.json", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var options = hostContext.Configuration.Get<EventMeshServerOptions>();
                    var serverBuilder = services.AddEventMeshServer(consensusNodeCallback: o =>
                    {
                        o.Port = options.Port;
                        o.ExposedUrl = options.ExposedUrl;
                        o.ExposedPort = options.ExposedPort;
                    })
                        .UseRocksDB(o =>
                        {
                            o.SubPath = $"node{options.Port}";
                        });
                    var pluginStore = serverBuilder.ServiceProvider.GetRequiredService<IPluginStore>();
                    var activePlugins = pluginStore.GetActivePlugins();
                    services.AddHostedService<EventMeshServerWorker>();
                    LoadProtocolPlugins(serverBuilder, activePlugins);
                    LoadSinkPlugins(serverBuilder, activePlugins);
                    LoadDiscoveryPlugins(serverBuilder, activePlugins);
                    serverBuilder.UseSinkRocksDB();
                }).Build();
await host.RunAsync();

static void LoadProtocolPlugins(ServerBuilder serverBuilder, IEnumerable<string> activePlugins)
{
    var pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.ProtocolsPluginSubPath);
    IEnumerable<string> pluginPaths = new string[0];
    if (Directory.Exists(pluginsDirectory)) pluginPaths = Directory.EnumerateDirectories(pluginsDirectory);
    var discoveredProtocolPlugins = pluginPaths.Select(p =>
    {
        if (PluginEntryDiscovery.TryExtract(p, activePlugins, new[] { typeof(IProxy) }, out IDiscoveredPlugin discoveredPlugin)) return discoveredPlugin;
        return null;
    }).Where(p => p != null);
    foreach (var discoveredPlugin in discoveredProtocolPlugins) discoveredPlugin.Load(serverBuilder);
    serverBuilder.Services.AddHostedService<EventMeshProxyWorker>();
}

static void LoadSinkPlugins(ServerBuilder serverBuilder, IEnumerable<string> activePlugins)
{
    var pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.SinksPluginSubPath);
    IEnumerable<string> pluginPaths = new string[0];
    if (Directory.Exists(pluginsDirectory)) pluginPaths = Directory.EnumerateDirectories(pluginsDirectory);
    var discoveredSinkPlugins = pluginPaths.Select(p =>
    {
        if (PluginEntryDiscovery.TryExtract(p, activePlugins, new[] { typeof(ISinkJob) }, out IDiscoveredPlugin discoveredPlugin)) return discoveredPlugin;
        return null;
    }).Where(p => p != null);
    foreach (var discoveredPlugin in discoveredSinkPlugins) discoveredPlugin.Load(serverBuilder);
    serverBuilder.Services.AddHostedService<EventMeshServerSinkWorker>();
}

static void LoadDiscoveryPlugins(ServerBuilder serverBuilder, IEnumerable<string> activePlugins)
{
    var pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.DiscoveriesPluginSubPath);
    IEnumerable<string> pluginPaths = new string[0];
    if (Directory.Exists(pluginsDirectory)) pluginPaths = Directory.EnumerateDirectories(pluginsDirectory);
    var discoveredDiscoveryPlugins = pluginPaths.Select(p =>
    {
        if (PluginEntryDiscovery.TryExtract(p, activePlugins, new [] { typeof(IClusterStore) }, out IDiscoveredPlugin discoveredPlugin)) return discoveredPlugin;
        return null;
    }).Where(p => p != null);
    foreach (var discoveredPlugin in discoveredDiscoveryPlugins) discoveredPlugin.Load(serverBuilder);
}