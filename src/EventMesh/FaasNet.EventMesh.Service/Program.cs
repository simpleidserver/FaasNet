using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
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
    await client.Ping(5000);

/*
Console.WriteLine("Press any key to add a VPN");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
    await client.AddVpn("VPN", "description");

Console.WriteLine("Press any key to get all the VPN");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    var vpnResult = await client.GetAllVpn();
    foreach(var vpn in vpnResult.Vpns)
        Console.WriteLine($"Name '{vpn.Id}', Description '{vpn.Description}'");
}

Console.WriteLine("Press any key to add a client");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
    await client.AddClient("publishClientId", "VPN", new List<ClientPurposeTypes> { ClientPurposeTypes.PUBLISH });

Console.WriteLine("Press any key to display all the clients");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
{
    var clientResult = await client.GetAllClient();
    foreach(var cl in clientResult.Clients)
        Console.WriteLine($"ClientId '{cl.Id}', Vpn '{cl.Vpn}', Purposes '{string.Join(",", cl.Purposes.Select(p => p.ToString()))}'");
}
*/

Console.WriteLine("Press any key to add a topic");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
    await client.AddTopic("topic1");

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
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
    await client.PublishMessage("topic1", "sessionId", cloudEvent);

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