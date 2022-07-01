using FaasNet.Common;
using FaasNet.DHT.Kademlia.Client;
using FaasNet.DHT.Kademlia.Core;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        private static IDHTPeerFactory _peerFactory;
        private static ICollection<IDHTPeer> _peers;
        private const int ROOT_NODE_PORT = 51;
        private static int CURRENT_NODE_PORT = 52;

        public static int Main(string[] args)
        {
            _peers = new List<IDHTPeer>();
            _peerFactory = new ServerBuilder().AddDHTKademlia().ServiceProvider.GetService(typeof(IDHTPeerFactory)) as IDHTPeerFactory;
            var rootNode = _peerFactory.Build();
            _peers.Add(rootNode);
            rootNode.Start(1, "localhost", ROOT_NODE_PORT, CancellationToken.None);
            var line = string.Empty;
            do
            {
                Console.WriteLine("add-peer: Add peer");
                Console.WriteLine("q: Exit");
                line = Console.ReadLine();
                if(line == "add-peer") AddPeer();
            }
            while (line != "q");

            Console.WriteLine("Press enter to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static void AddPeer()
        {
            Console.WriteLine("Enter a peer identifier");
            var peerId = long.Parse(Console.ReadLine());
            var node = _peerFactory.Build();
            node.Start(peerId, "localhost", CURRENT_NODE_PORT, CancellationToken.None);
            using(var client = new KademliaClient("localhost", CURRENT_NODE_PORT))
            {
                client.Join(1, "localhost", ROOT_NODE_PORT).Wait();
            }
            _peers.Add(node);
            CURRENT_NODE_PORT++;
        }
    }
}
