using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;

var clusterNodes = new ConcurrentBag<ClusterPeer>();
await BuildAndStartNode(5000, clusterNodes, 30000);
await BuildAndStartNode(5001, clusterNodes, 40000);

Console.WriteLine("Press any key to ping the application");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    await client.Ping(5000);
}

Console.WriteLine("Press any key to get all the nodes");
Console.ReadLine();
using (var client = PeerClientFactory.Build<PartitionClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    var nodesResult = await client.GetAllNodes(5000);
    foreach (var node in nodesResult.Nodes) Console.WriteLine($"{node.Url}:{node.Port}");
}

Console.WriteLine("Press any key to add a VPN");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    await Retry(async () => await client.AddVpn("VPN", "description"), (r) =>
    {
        if (r.Success) return true;
        DisplayError(Enum.GetName(typeof(AddVpnErrorStatus), r.Status));
        return false;
    });
}

Console.WriteLine("Press any key to get all the VPN");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    var vpnResult = await client.GetAllVpn();
    foreach(var vpn in vpnResult.Vpns)
    {
        Console.WriteLine($"Name '{vpn.Id}', Description '{vpn.Description}'");
    }
}

AddClientResult publishClient;
Console.WriteLine("Press any key to add a client");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    publishClient = await Retry(async() => await client.AddClient("clientId", "VPN", new List<ClientPurposeTypes> { ClientPurposeTypes.PUBLISH, ClientPurposeTypes.SUBSCRIBE }), (r) =>
    {
        if (r.Success) return true;
        DisplayError(Enum.GetName(typeof(AddClientErrorStatus), r.Status.Value));
        return false;
    });
}

Console.WriteLine("Press any key to display all the clients");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    var clientResult = await client.GetAllClient();
    foreach(var cl in clientResult.Clients)
    {
        Console.WriteLine($"ClientId '{cl.Id}', Vpn '{cl.Vpn}', Purposes '{string.Join(",", cl.Purposes.Select(p => p.ToString()))}'");
    }
}

Console.WriteLine("Press any key to add a queue");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    await Retry(async() => await client.AddQueue("queue", "topic1"), (c) =>
    {
        if (c.Status == AddQueueStatus.SUCCESS) return true;
        DisplayError(Enum.GetName(typeof(AddQueueStatus), c.Status));
        return false;
    });
}

Console.WriteLine("Press any key to create a session used for publishing");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    var pubSession = await client.CreatePubSession("clientId", publishClient.ClientSecret);
    Console.WriteLine("Press any key to publish a message");
    Console.ReadLine();
    var cloudEvent = new CloudEvent
    {
        Type = "com.github.pull.create",
        Source = new Uri("https://github.com/cloudevents/spec/pull"),
        Subject = "123",
        Id = "A234-1234-1234",
        Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
        DataContentType = "application/json",
        Data = "testttt",
        ["comexampleextension1"] = "value"
    };
    var result = await Retry(async() => await pubSession.PublishMessage("topic1", cloudEvent, timeoutMS: 200000), (r) =>
    {
        if (r.Status == PublishMessageStatus.SUCCESS) return true;
        DisplayError(Enum.GetName(typeof(PublishMessageStatus), r.Status));
        return false;
    });
    foreach(var queueName in result.QueueNames)
    {
        Console.WriteLine($"Message has been published into {queueName}");
    }
}

Console.WriteLine("Press any key to create a session used for subscription");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    var subSession = await client.CreateSubSession("clientId", publishClient.ClientSecret, "queue");
    Console.WriteLine("Press any key to read a message");
    Console.ReadLine();
    var readMessage = await Retry(() => subSession.ReadMessage(1, timeoutMS: 2000000), (r) =>
    {
        if (r == null) return false;
        if (r.Status == ReadMessageStatus.SUCCESS) return true;
        DisplayError(Enum.GetName(typeof(ReadMessageStatus), r.Status));
        return false;
    });
    Console.WriteLine($"Message received : {readMessage.Message.Data}");
}

Console.WriteLine("Press any key to quit the application");
Console.ReadLine();

static async Task<IPeerHost> BuildAndStartNode(int port, ConcurrentBag<ClusterPeer> clusterNodes, int startPeerPort = 30000)
{
    PartitionedNodeHostFactory nodeHostFactory = PartitionedNodeHostFactory.New(options: p =>
    {
        p.Port = port;
    }, nodeOptions: no =>
    {
        no.StartPeerPort = startPeerPort;
    }, clusterNodes: clusterNodes)
        .UseUDPTransport()
        .UseEventMesh();
    var node = nodeHostFactory.Build();
    await node.Start();
    return node;
}

static async Task<T> Retry<T>(Func<Task<T>> callback, Func<T, bool> continueCallback, int nbRetry = 0) where T : class
{
    if (nbRetry == 5) throw new InvalidOperationException("Too many retry");
    var result = await callback();
    if (!continueCallback(result))
    {
        Thread.Sleep(2000);
        nbRetry++;
        return await Retry(callback, continueCallback, nbRetry);
    }

    return result;
}

static void DisplayError(string msg)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(msg);
    Console.ForegroundColor = ConsoleColor.White;
}