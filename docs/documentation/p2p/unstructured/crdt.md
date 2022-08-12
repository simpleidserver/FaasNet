# Conflict-free Replicated Data Type (CRDT)

## Architecture

CRDT is a data structure that is replicated across multiple Peers in a Network.

There are three types of CRDTs

| Type              | Description                                                                                                                   |
| ----------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| State-based       | Each Peer propagate their full local state to other Peers                                                                     |
| Operation-based   | Each Peer propagate only Update operations to other Peers                                                                     |
| Delta             | During the synchronization process, two Peers are going to compare their local Data Structure and exchange the computed delta |

## Entity Types

CRDT support different kinds of Entity 

| Entity Type | Description                  |
| ----------- | ---------------------------- |
| GCounter    | Grow only Counter            |
| PNCounter   | Positive-Negative Counter    |
| GSet        | Grow-only Set                |
| 2PSet       | Two-Phase set                |
| LWWEltSet   | Last-Write-Wins element set  |
| ORSet       | Observed-Remove Set          |
| SEQ         | Sequence CRDTs               |

## Library

In this tutorial, we are going to explain how to use the C# library to have an up and running CRDT(Delta) server.

Open a command prompt and create a console application.

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new console -n CRDTServer
```

Install the Nuget package `FaasNet.CRDT.Core`.

```
cd CRDTServer

dotnet add package FaasNet.CRDT.Core
```

Add the console application into the Visual Studio Solution.

```
cd ../..
dotnet new sln -n QuickStart
dotnet sln add ./src/CRDTServer/CRDTServer.csproj
```

Open the Visual Studio Solution and edit the `Program.cs` file.

Add a procedure `LaunchCRDTPeer(ClusterPeers, Port, PeerId)`, it will be used to start one CRDT Peer.

```
private static void LaunchCRDTPeer(ConcurrentBag<ClusterPeer> clusterPeers, int port = 5001, string peerId = "peerId")
{
    var gcounter = new GCounter(peerId);
    var firstSerializedEntity = new CRDTEntitySerializer().Serialize("nb_customers", gcounter);;
    var entities = new ConcurrentBag<SerializedEntity>
    {
        firstSerializedEntity
    };
    var peerHost = PeerHostFactory.New(o => {
            o.Port = port;
            o.PeerId = peerId;
        }, clusterPeers)
        .UseUDPTransport()
        .AddCRDTProtocol(entities)
        .Build();
    peerHost.Start();
}
```

Add a new procedure `Increment()`, it will be used to increment the entity `nb_customers`.

```
private static void Increment()
{
	using (var crdtClient = new UDPCRDTClient("localhost", 5001))
	{
		crdtClient.IncrementGCounter("nb_customers", 2).Wait();
	}
}
```

Add a procedure `Display()`, it will be used to display the value of the entity `nb_customers`.

```
private static void Display()
{
	using (var crdtClient = new UDPCRDTClient("localhost", 5001))
	{
		var result = crdtClient.Get("nb_customers").Result;
		Console.WriteLine($"nb_customers = {result}");
	}
}
```

Add the following code to add two peers, increment the GCounter `nb_customers` and display its value.

```
LaunchCRDTPeer("localhost", 5001, "peerId1");
LaunchCRDTPeer("localhost", 5002, "peerId2");
Increment();
Display();
```