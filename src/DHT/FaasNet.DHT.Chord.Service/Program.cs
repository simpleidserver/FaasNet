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
                Console.WriteLine("add-peer: Add peer");
                Console.WriteLine("add-key: Add key");
                Console.WriteLine("stop-peer: Stop peer");
                Console.WriteLine("fingers: Display fingers");
                Console.WriteLine("data: Display data");
                Console.WriteLine("q: Exit");
                line = Console.ReadLine();
                if(line == "add-peer") AddPeer();
                if (line == "add-key") AddKey();
                if (line == "stop-peer") StopPeer();
                if (line == "fingers") DisplayFingers();
                if (line == "data") DisplayData();
            }
            while (line != "q");

            Console.WriteLine("Press enter to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static void AddPeer()
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

        private static void AddKey()
        {
            Console.WriteLine("Enter a key");
            var key = long.Parse(Console.ReadLine());
            Console.WriteLine("Enter a value");
            var value = Console.ReadLine();
            using (var chordClient = new ChordClient("localhost", ROOT_NODE_PORT))
            {
                chordClient.AddKey(key, value);
            }
        }

        private static void DisplayFingers()
        {
            foreach(var peer in _peers)
            {
                var peerInfo = peer.PeerInfoStore.Get();
                Console.WriteLine($"Peer Identifier {peerInfo.Peer.Id}, Predecessor {peerInfo.PredecessorPeer?.Id}, Successor {peerInfo.SuccessorPeer?.Id}");
                Console.WriteLine("Finger tables");
                foreach(var finger in peerInfo.Fingers)
                {
                    Console.WriteLine($"Start {finger.Start}, End {finger.End}, Id {finger.Peer.Id}");
                }

                Console.WriteLine();
            }
        }

        private static void DisplayData()
        {
            foreach (var peer in _peers)
            {
                var peerInfo = peer.PeerInfoStore.Get();
                var allData = peer.PeerDataStore.GetAll();
                Console.WriteLine($"Peer Identifier {peerInfo.Peer.Id}, Predecessor {peerInfo.PredecessorPeer?.Id}, Successor {peerInfo.SuccessorPeer?.Id}");
                Console.WriteLine("Data");
                foreach (var data in allData)
                {
                    Console.WriteLine($"Key {data.Id}, Value {data.Value}");
                }

                Console.WriteLine();
            }
        }

        private static void StopPeer()
        {
            Console.WriteLine("Enter the peer identifier");
            var id = int.Parse(Console.ReadLine());
            var peer = _peers.First(p => p.PeerInfoStore.Get().Peer.Id == id);
            peer.Stop();
            _peers.Remove(peer);
        }
    }
}
