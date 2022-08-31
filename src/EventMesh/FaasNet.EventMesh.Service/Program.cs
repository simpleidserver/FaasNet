using FaasNet.EventMesh.Client;
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

Console.WriteLine("Press any key to add a VPN");
Console.ReadLine();
using (var client = PeerClientFactory.Build<EventMeshClient>("localhost", 5000, ClientTransportFactory.NewUDP()))
    await client.AddVpn("VPN");

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