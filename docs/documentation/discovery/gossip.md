# Gossip

Gossip or epidemic protocol is a process of P2P communication that is based on epidemics spread.

Most of the time the protocol is used to propagate Peer Informations to all the other Peers, it helps Peers to discover other Peers present in the Network.

When the protocol is enabled, Peer informations are stored in a local storage such as a configuration file or a database.
Shared storage like ETCD are not used.

In general, Gossip protocol is used to exchange state messages between two or more Peers.
A state is overridden by the latest message received, so there is a possibility of losing state.
The protocol must be used to exchange immutable information like metadata.

Gossip protocol is used in some popular software

| Software    | Usage                                                                    |
| ----------- | ------------------------------------------------------------------------ |
| Cassandra   | Uses Gossip protocol for the group membership and failure detection      |
| Consul      | Uses SWIM Gossip protocol for the group membership and failure detection |
| CockroachDB | Uses Gossip protocol to propagate Peer metadata                          |
| Blockchain  | Uses Gossip protocol for group membership and sending ledger metadata    |

## Quick start

In this tutorial, we are going to explain how to use the C# library to have an up and running UDP Peer where Gossip protocol is used for Peers discovery.

Open a command prompt and create a console application.

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new console -n DiscoveryGossip
```

Install the Nuget package `FaasNet.Discovery.Gossip.Core`

```
cd DiscoveryGossip

dotnet add package FaasNet.Discovery.Gossip.Core
```

Add the console application into the Visual Studio Solution

```
cd ..\..
dotnet new sln -n QuickStart
dotnet sln add ./src/DiscoveryGossip/DiscoveryGossip.csproj
```

Add a function `IPeerHost LaunchGossipPeer(clusterPeers, port, PeerId)`.
It will be used to start one unstructured UDP Peer with Gossip protocol enabled.

```
private static IPeerHost LaunchGossipPeer(ConcurrentBag<ClusterPeer> clusterPeers, int port = 5001, string peerId = "peerId")
{
    var peerHost = PeerHostFactory.NewUnstructured(o => {
        o.Url = "localhost";
        o.Port = port;
        o.PeerId = peerId;
    }, clusterPeers)
        .UseUDPTransport()
        .UseGossipDiscovery()
        .Build();
    peerHost.Start();
    return peerHost;
}
```

Add a procedure `DisplayCluster()`. It will be used to display all the Peer informations.

```
private static void DisplayCluster()
{
    using (var gossipClient = new UDPGossipClient("localhost", 5002))
    {
        var peerInfos = gossipClient.Get().Result;
        foreach(var peerInfo in peerInfos)
        {
            Console.WriteLine($"Url = {peerInfo.Url}, Port = {peerInfo.Port}");
        }
    }
}
```

Add the following code to start one Peer and display the Peer informations.

```
LaunchGossipPeer(new ConcurrentBag<ClusterPeer>(), 5001, "seedId");
DisplayCluster();
```