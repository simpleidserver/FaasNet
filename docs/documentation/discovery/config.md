# Configuration file

Peer Informations can be persisted in a configuration file.

## Quick start

In this tutorial, we are going to explain how to use the C# library to have an up and running UDP Peer where Peer Informations are stored in the configuration file.

Open a command prompt and create a console application.

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new console -n DiscoveryConfig
```

Install the Nuget package `FaasNet.Discovery.Config`

```
cd DiscoveryConfig

dotnet add package FaasNet.Discovery.Config
```

Add the console application into the Visual Studio Solution

```
cd ..\..
dotnet new sln -n QuickStart
dotnet sln add ./src/DiscoveryConfig/DiscoveryConfig.csproj
```

Open the Visual Studio Solution and edit the `Program.cs` file.

Add a function `(IPeerHost, IServiceProvider) LaunchPeer(clusterPeers, port, PeerId)`.
It will be used to start one unstructured UDP Peer and the Peer Informations will be stored in the configuration file.

```
private static (IPeerHost, IServiceProvider) LaunchPeer(ConcurrentBag<ClusterPeer> clusterPeers, int port = 5001, string peerId = "peerId")
{
    var peerHostFactory = PeerHostFactory.NewUnstructured(o =>
    {
        o.Url = "localhost";
        o.Port = port;
        o.PeerId = peerId;
    }, clusterPeers)
        .UseUDPTransport()
        .UseDiscoveryConfig();
    var peerHost = peerHostFactory.BuildWithDI();
    peerHost.Item1.Start();
    return peerHost;
}
```

Add a procedure `DisplayCluster(ServiceProvider)`. It will be used to display all the Peer informations coming form the configuration file.

```
private static void DisplayCluster(IServiceProvider serviceProvider)
{
    var clusterStore = serviceProvider.GetRequiredService<IClusterStore>();
    var allNodes = clusterStore.GetAllNodes(CancellationToken.None).Result;
    foreach (var node in allNodes)
    {
        Console.WriteLine($"Url = {node.Url}, Port = {node.Port}");
    }
}
```

Add the following code to start one Peer and display the Peer informations.

```
var peerHost = LaunchPeer(new ConcurrentBag<ClusterPeer>(), 5001, "seedId");
DisplayCluster(peerHost.Item2);
```