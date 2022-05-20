using Amqp;
using FaasNet.Common;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Protocols.AMQP;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

var frame = new Frame { Channel = 4, Type = 0 };
var payload = frame.Serialize();
var buffer = new ByteBuffer(payload, 0, payload.Count(), 0);
Frame.Decode(buffer, out ushort channel);

var amqpServer = new AMQPServer();
amqpServer.Start();
Console.WriteLine("Press enter");
Console.ReadLine();

// Address address = new Address("amqp://guest:guest@localhost:5672");
// Connection connection = new Connection(address);
// Session session = new Session(connection);
/*
var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "hello",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);
}
*/

/*
const int seedPort = 4000;
int nbNode = 1;
var allNodes = new List<INodeHost> { BuildNodeHost(seedPort, true) };
await StartNodes(allNodes);
await DisplayMenu(allNodes);
Console.WriteLine("Press Enter to quit the application");
Console.ReadLine();
await StopNodes(allNodes);


async Task StartNodes(IEnumerable<INodeHost> allNodes)
{
    for (int i = 0; i < allNodes.Count(); i++) await StartNode(allNodes.ElementAt(i), i);
}

async Task StartNode(INodeHost node, int nbNodes)
{
    await node.Start(CancellationToken.None);
    foreach (var peer in node.Peers)
    {
        peer.PeerIsFollower += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is follower");
        peer.PeerIsCandidate += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is candidate");
        peer.PeerIsLeader += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is leader");
    }

    node.NodeStarted += (s, e) =>
    {
        if (node.Port == seedPort) return;
        using (var gossipClient = new GossipClient("localhost", seedPort))
        {
            gossipClient.JoinNode("localhost", node.Port);
        }
    };
}

async Task StopNodes(IEnumerable<INodeHost> allNodes)
{
    foreach (var node in allNodes) await node.Stop();
}

async Task DisplayMenu(ICollection<INodeHost> nodes)
{
    var continueExecution = true;
    do
    {
        Console.WriteLine("- Enter 'Q' to stop execution");
        Console.WriteLine("- Enter 'addnode' to add a node");
        Console.WriteLine("- Enter 'states' to display the states");
        Console.WriteLine("- Enter 'peers' to display the peers");
        Console.WriteLine("- Enter 'addvpn' to add a VPN");
        Console.WriteLine("- Enter 'addclient' to add a client");
        Console.WriteLine("- Enter 'publishmsg' to publish a message");
        Console.WriteLine("- Enter 'persistedsub' to subscribe to a group identifier");
        Console.WriteLine("- Enter 'directsub' to subscribe to a topic");
        string menuId = Console.ReadLine();
        continueExecution = menuId != "Q";
        if(menuId == "addnode")
        {
            int currentPort = seedPort + nbNode;
            var newHost = BuildNodeHost(currentPort, false);
            await StartNode(newHost, nbNode);
            allNodes.Add(newHost);
            nbNode++;
            continue;
        }

        if (menuId == "states")
        {
            foreach (var node in nodes)
            {
                var entityTypes = await node.NodeStateStore.GetAllLastEntityTypes(CancellationToken.None);
                foreach (var entityType in entityTypes)
                {
                    Console.WriteLine($"Port = {node.Port}, Type = {entityType.EntityType}, Version = {entityType.EntityVersion}, Value = {entityType.Value}");
                }
            }

            continue;
        }

        if (menuId == "peers")
        {
            foreach (var node in allNodes)
            {
                foreach (var peer in node.Peers)
                {
                    Console.WriteLine($"Node '{node.NodeId}', Peer '{peer.PeerId}', TermId '{peer.Info.TermId}', TermIndex '{peer.Info.ConfirmedTermIndex}', State '{peer.State}");
                }
            }
            continue;
        }

        if (menuId == "addvpn")
        {
            Console.WriteLine("Enter the VPN");
            string vpn = Console.ReadLine();
            var eventMeshClient = new EventMeshClient("localhost", seedPort);
            await eventMeshClient.AddVpn(vpn, CancellationToken.None);
            continue;
        }

        if (menuId == "addclient")
        {
            Console.WriteLine("Enter the VPN");
            string vpn = Console.ReadLine();
            Console.WriteLine("Enter the client identifier");
            string clientId = Console.ReadLine();
            var eventMeshClient = new EventMeshClient("localhost", seedPort);
            await eventMeshClient.AddClient(vpn, clientId, new List<UserAgentPurpose> { UserAgentPurpose.SUB, UserAgentPurpose.PUB }, CancellationToken.None);
            continue;
        }

        if (menuId == "publishmsg")
        {
            Console.WriteLine("Enter the VPN");
            var vpn = Console.ReadLine();
            Console.WriteLine("Enter the client identifier");
            var clientIdentifier = Console.ReadLine();
            var eventMeshClient = new EventMeshClient("localhost", seedPort);
            var session = await eventMeshClient.CreatePubSession(vpn, clientIdentifier, CancellationToken.None);
            await session.Publish("person.created", new { firstName = "firstName" }, CancellationToken.None);
            continue;
        }

        if (menuId == "persistedsub")
        {
            Console.WriteLine("Enter the VPN");
            var vpn = Console.ReadLine();
            Console.WriteLine("Enter the client identifier");
            var clientIdentifier = Console.ReadLine();
            Console.WriteLine("Enter the group identifier");
            var groupId = Console.ReadLine();
            var eventMeshClient = new EventMeshClient("localhost", seedPort);
            var session = await eventMeshClient.CreateSubSession(vpn, clientIdentifier, CancellationToken.None);
            await session.PersistedSubscribe("person.created", groupId, (ce) =>
            {
                Console.WriteLine("Persisted sub");
            }, CancellationToken.None);
            continue;
        }

        if (menuId == "directsub")
        {
            Console.WriteLine("Enter the VPN");
            var vpn = Console.ReadLine();
            Console.WriteLine("Enter the client identifier");
            var clientIdentifier = Console.ReadLine();
            var eventMeshClient = new EventMeshClient("localhost", seedPort);
            var session = await eventMeshClient.CreateSubSession(vpn, clientIdentifier, CancellationToken.None);
            session.DirectSubscribe("person.created", (ce) =>
            {
                Console.WriteLine("Direct sub");
            }, CancellationToken.None);
            continue;
        }
    }
    while (continueExecution);
}

INodeHost BuildNodeHost(int port, bool isSeed = false)
{
    var serverBuilder = new ServiceCollection()
        .AddEventMeshServer(o => o.Port = port)
        .UseRocksDB(o => { o.SubPath = $"node{port}"; });
    // if (isSeed) serverBuilder.SetNodeStates(new ConcurrentBag<NodeState> { new ClusterNode { Port = 4000, Url = "localhost" }.ToNodeState() });
    var serviceProvider = serverBuilder.Services
        // .AddLogging(l => l.AddConsole())
        .BuildServiceProvider();
    if(isSeed)
    {
        var nodeStateStore = serviceProvider.GetRequiredService<INodeStateStore>();
        nodeStateStore.Add(new ClusterNode { Port = 4000, Url = "localhost" }.ToNodeState());
    }

    return serviceProvider.GetRequiredService<INodeHost>();
}
*/