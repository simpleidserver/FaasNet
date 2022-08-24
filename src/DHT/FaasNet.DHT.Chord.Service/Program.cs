using FaasNet.Common;
using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Core.Stores;
using FaasNet.Peer;
using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        private static ICollection<(IPeerHost, IServiceProvider)> _peers;
        private static int CURRENT_NODE_PORT = 57;
        private const int ROOT_NODE_PORT = 51;
        private const int DIM_FINGER_TABLE = 4;

        public static int Main(string[] args)
        {
            _peers = new List<(IPeerHost, IServiceProvider)>();
            AddRootPeer();
            var line = string.Empty;
            do
            {
                Console.WriteLine("add-peer: Add peer");
                Console.WriteLine("add-key: Add key");
                Console.WriteLine("get-key: Get key");
                Console.WriteLine("stop-peer: Stop peer");
                Console.WriteLine("fingers: Display fingers");
                Console.WriteLine("data: Display data");
                Console.WriteLine("q: Exit");
                line = Console.ReadLine();
                if(line == "add-peer") AddPeer();
                if (line == "add-key") AddKey();
                if (line == "get-key") GetKey();
                if (line == "stop-peer") StopPeer();
                if (line == "fingers") DisplayFingers();
                if (line == "data") DisplayData();
            }
            while (line != "q");
            Console.WriteLine("Press enter to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static async void AddRootPeer()
        {
            var rootNode = PeerHostFactory.NewStructured(o =>
            {
                o.Port = ROOT_NODE_PORT;
                o.Url = "localhost";
            }).UseTCPTransport().AddDHTChordProtocol().BuildWithDI();
            _peers.Add(rootNode);
            await rootNode.Item1.Start();
            using (var firstClient = PeerClientFactory.Build<ChordClient>("localhost", ROOT_NODE_PORT, ClientTransportFactory.NewTCP()))
            {
                await firstClient.Create(DIM_FINGER_TABLE, timeoutMS: 5000);
            }
        }

        private static async void AddPeer()
        {
            var node = PeerHostFactory.NewStructured(o =>
            {
                o.Port = CURRENT_NODE_PORT;
                o.Url = "localhost";
            }).UseTCPTransport().AddDHTChordProtocol().BuildWithDI();
            await node.Item1.Start();
            using (var secondClient = PeerClientFactory.Build<ChordClient>("localhost", CURRENT_NODE_PORT, ClientTransportFactory.NewTCP()))
            {
                await secondClient.Join("localhost", ROOT_NODE_PORT, 5000);
            }

            _peers.Add(node);
            CURRENT_NODE_PORT++;
        }

        private static async void AddKey()
        {
            Console.WriteLine("Enter a key");
            var key = long.Parse(Console.ReadLine());
            Console.WriteLine("Enter a value");
            var value = Console.ReadLine();
            using (var chordClient = PeerClientFactory.Build<ChordClient>("localhost", ROOT_NODE_PORT, ClientTransportFactory.NewTCP()))
            {
                await chordClient.AddKey(key, value, timeoutMS: 5000);
            }
        }

        private static async void GetKey()
        {
            Console.WriteLine("Enter a key");
            var key = long.Parse(Console.ReadLine());
            using(var chordClient = PeerClientFactory.Build<ChordClient>("localhost", ROOT_NODE_PORT, ClientTransportFactory.NewTCP()))
            {
                var value = await chordClient.GetKey(key, timeoutMS: 5000);
                Console.WriteLine($"Key {key}, Value {value}");
            }
        }

        private static void DisplayFingers()
        {
            foreach (var rec in _peers)
            {
                var peerInfoStore = rec.Item2.GetService(typeof(IDHTPeerInfoStore)) as IDHTPeerInfoStore;
                var peerInfo = peerInfoStore.Get();
                Console.WriteLine($"Peer Identifier {peerInfo.Peer.Id}, Predecessor {peerInfo.PredecessorPeer?.Id}, Successor {peerInfo.SuccessorPeer?.Id}");
                Console.WriteLine("Finger tables");
                foreach (var finger in peerInfo.Fingers)
                {
                    Console.WriteLine($"Start {finger.Start}, End {finger.End}, Id {finger.Peer.Id}");
                }

                Console.WriteLine();
            }
        }

        private static void DisplayData()
        {
            foreach (var rec in _peers)
            {
                var peerInfoStore = rec.Item2.GetService(typeof(IDHTPeerInfoStore)) as IDHTPeerInfoStore;
                var peerDataStore = rec.Item2.GetService(typeof(IPeerDataStore)) as IPeerDataStore;
                var peerInfo = peerInfoStore.Get();
                var allData = peerDataStore.GetAll();
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
            var peer = _peers.First(p =>
            {
                var peerInfo = p.Item2.GetService(typeof(IDHTPeerInfoStore)) as IDHTPeerInfoStore;
                return peerInfo.Get().Peer.Id == id;
            });
            peer.Item1.Stop();
            _peers.Remove(peer);
        }
    }
}
