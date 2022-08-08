# ETCD

ETCD is a distributed key-value store. It can be used by a P2P Network to store Peer Informations like Url, Port and Protocol.

## Quick start

In this tutorial we are going to explain how to have an up and running UDP Peer where Peer informations are stored in ETCD.

### Install ETCD

Ensure Docker is installed on your local machine. Run an ETCD server instance by executing the following command.

```
docker run -d --name Etcd-server --publish 23790:2379 --env ALLOW_NONE_AUTHENTICATION=yes bitnami/etcd:latest
```

An ETCD server instance will be launched and will listen the port `23790/TCP`.

### Create and Launch a Peer

Open a command prompt and create a console application

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new console -n DiscoveryEtcd
```

Install the Nuget package `FaasNet.Discovery.Etcd`

```
cd DiscoveryEtcd

dotnet add package FaasNet.Discovery.Etcd
```

Add the console application into the Visual Studio Solution

```
cd ..\..
dotnet new sln -n QuickStart
dotnet sln add ./src/DiscoveryEtcd/DiscoveryEtcd.csproj
```

Add a function `(IPeerHost, IServiceProvider) LaunchPeer(clusterPeers, port, PeerId)`.
It will be used to start one unstructured UDP Peer and the Peer Informations will be stored in ETCD.

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
		.UseDiscoveryEtcd();
	var peerHost = peerHostFactory.BuildWithDI();
	peerHost.Item1.Start();
	return peerHost;
}
```

Add a procedure `DisplayCluster(ServiceProvider)`. It will be used to display all the Peer informations coming form ETCD.

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