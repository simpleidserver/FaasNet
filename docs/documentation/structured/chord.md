# CHORD

## Architecture

The CHORD protocol uses SHA-1 as consistent hash function to assin a `m`-bit identifier to each node and each key.

The `m` is an integer which should be chosen big enough to make the probability that two nodes or two keys receive the same identifier negligible.

The Hash function calculates the key identifier by hashing the key, and the node identifier by hashing the IP Address of the node.

The key and the node identifiers are arranged on an identifier circle of size `2^m` called the CHORD ring.
The identifiers on the CHORD ring are numbered from 0 to 2^m-1. A key is assigned to a node whose identifier is equal to or greater than the identifier of the key.
The node is called the successor node of k, and is the first node clockwise from k on the circle.

![CHORD Ring](images/chord-ring.png)

This figure shows a CHORD ring with m = 6, 10 nodes and 5 keys.
Sice the successor of K10 is N14, K10 is located at N14.

## Library

In this tutorial, we are going to explain how to use the C# library to have an up and running CHORD server.

Open a command prompt and create a console application.

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new console -n ChordServer
```

Install the Nuget package `FaasNet.DHT.Chord.Client` and `FaasNet.DHT.Chord.Core`.

```
cd ChordServer

dotnet add package FaasNet.DHT.Chord.Client
FaasNet.DHT.Chord.Core
```

Add the console application into the Visual Studio Solution.

```
cd ../..
dotnet new sln -n QuickStart
dotnet sln add ./src/ChordServer/ChordServer.csproj
```

Open the Visual Studio Solution and edit the `Program.cs` file.

Add a new procedure `StartChordRing`, it will be used to start the CHORD ring.

```
private static void StartChordRing()
{
    var peerFactory = new ServerBuilder().AddDHTChord().ServiceProvider.GetService(typeof(IDHTPeerFactory)) as IDHTPeerFactory;
    var rootPeer = peerFactory.Build();
    rootPeer.Start("localhost", 51, CancellationToken.None);
    using (var client = new ChordClient("localhost", 51))
    {
        client.Create(4);
    }
}
```

Add a new procedure `AddNode(int port)`, it will be used to add a node into the ring.

```
private static void AddNode(int port)
{
    var peerFactory = new ServerBuilder().AddDHTChord().ServiceProvider.GetService(typeof(IDHTPeerFactory)) as IDHTPeerFactory;
    var peer = peerFactory.Build();
    peer.Start("localhost", port, CancellationToken.None);
    using (var client = new ChordClient("localhost", port))
    {
        client.Join("localhost", 51);
    }
}
```


Add a new procedure `PersistKey(long key, string value)`, it will be used to persist a Key and its Value into the ring.

```
private static void PersistKey(long key, string value)
{
    using (var chordClient = new ChordClient("localhost", 51))
    {
        chordClient.AddKey(key, value);
    }
}
```

Add a new procedure `GetKey(long key)`, it will be used to get the value of the key from the ring.

```
private static string GetKey(long key)
{
    using (var chordClient = new ChordClient("localhost", 51))
    {
        return chordClient.GetKey(key);
    }
}
```

Add the following code to add two peers into the ring, publish a key with its value and finally display the value of the key.

```
StartChordRing();
AddNode(57);
AddNode(58);
PersistKey(8, "Hello");
Console.WriteLine($"Key 8 is stored with the value {GetKey(8)}");
```