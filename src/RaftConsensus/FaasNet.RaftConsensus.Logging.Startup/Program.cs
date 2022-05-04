using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

// Add PEERSTORE.
// Add logstore into file.
// Add client.
var env = GetArgument("env", args);
if (string.IsNullOrWhiteSpace(env)) env = "firstNode";

var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.{env}.json")
    .Build();

var hostSettings = configuration.GetSection("host").Get<HostSettings>();

var node = BuildNodeHost(new ConcurrentBag<PeerInfo>(hostSettings.PeerInfos), hostSettings.Port, new ConcurrentBag<ClusterNode>(hostSettings.Nodes));
await node.Start(CancellationToken.None);

Console.WriteLine("Press Enter to quit the application");
Console.ReadLine();

static INodeHost BuildNodeHost(ConcurrentBag<PeerInfo> peers, int port, ConcurrentBag<ClusterNode> clusterNodes)
{
    var serviceProvider = new ServiceCollection()
        .AddConsensusPeer(o => o.Port = port)
        .SetPeers(peers)
        .SetClusterNodes(clusterNodes)
        .Services
        .AddLogging(l => l.AddConsole())
        .BuildServiceProvider();
    return serviceProvider.GetService<INodeHost>();
}

static string GetArgument(string key, string[] arr)
{
    if (arr == null) return string.Empty;
    var result = arr.FirstOrDefault(a => a.StartsWith(key));
    if (result == null) return string.Empty;
    return result.Split('=').Last();
}

class HostSettings
{
    public string Url { get; set; }
    public int Port { get; set; }
    public List<PeerInfo> PeerInfos { get; set; }
    public List<ClusterNode> Nodes { get; set; }
}