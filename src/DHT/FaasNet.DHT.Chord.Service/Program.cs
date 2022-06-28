using FaasNet.Common;
using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Core;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        private static IDHTPeerFactory _peerFactory;
        private static ICollection<IDHTPeer> _peers;
        private static int CURRENT_NODE_PORT = 57;
        private const int ROOT_NODE_PORT = 51;
        private const int DIM_FINGER_TABLE = 4;

        public static int Main(string[] args)
        {
            // FIX PROBLEM !!!
            _peers = new List<IDHTPeer>();
            _peerFactory = new ServerBuilder().AddDHTChord().ServiceProvider.GetService(typeof(IDHTPeerFactory)) as IDHTPeerFactory;
            var rootNode = _peerFactory.Build();
            _peers.Add(rootNode);
            rootNode.Start("localhost", ROOT_NODE_PORT, CancellationToken.None);
            using (var firstClient = new ChordClient("localhost", ROOT_NODE_PORT))
            {
                firstClient.Create(DIM_FINGER_TABLE);
            }

            var line = string.Empty;
            do
            {
                Console.WriteLine("add: Add node");
                Console.WriteLine("fingers: Display fingers");
                Console.WriteLine("q: Exit");
                line = Console.ReadLine();
                if(line == "add") AddNode();
                if (line == "fingers") DisplayFingers();
            }
            while (line != "q");

            Console.WriteLine("Press enter to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static void AddNode()
        {
            var node = _peerFactory.Build();
            node.Start("localhost", CURRENT_NODE_PORT, CancellationToken.None);
            using (var secondClient = new ChordClient("localhost", CURRENT_NODE_PORT))
            {
                secondClient.Join("localhost", ROOT_NODE_PORT);
            }

            _peers.Add(node);
            CURRENT_NODE_PORT++;
        }

        private static void DisplayFingers()
        {
            foreach(var peer in _peers)
            {
                var peerInfo = peer.PeerInfoStore.Get();
                Console.WriteLine($"Peer Identifier {peerInfo.Peer.Id}, Predecessor {peerInfo.PredecessorPeer?.Id}, Successor {peerInfo.SuccessorPeer.Id}");
                Console.WriteLine("Finger tables");
                foreach(var finger in peerInfo.Fingers)
                {
                    Console.WriteLine($"Start {finger.Start}, End {finger.End}, Id {finger.Peer.Id}");
                }

                Console.WriteLine();
            }
        }
    }
}
