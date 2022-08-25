using FaasNet.EventMesh.Client;
using FaasNet.Partition;
using FaasNet.Peer.Client;

PartitionedNodeHostFactory nodeHostFactory = PartitionedNodeHostFactory.New(options: p =>
{
    p.Port = 5000;
})
    .UseUDPTransport()
    .UseEventMesh();
var node = nodeHostFactory.Build();
await node.Start();

Console.WriteLine("Press any key to ping the application");
Console.ReadLine();

using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
    await client.Ping(5000);

Console.WriteLine("Press any key to quit the application");
Console.ReadLine();