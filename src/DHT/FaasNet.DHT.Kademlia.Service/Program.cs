using FaasNet.DHT.Kademlia.Client;
using FaasNet.DHT.Kademlia.Core.Stores;
using FaasNet.Peer;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        private static ICollection<(IPeerHost, IServiceProvider)> _peers;
        private const long ROOT_PEER_ID = 1;
        private const int ROOT_NODE_PORT = 50;
        private static int CURRENT_NODE_PORT = 60;

        public static int Main(string[] args)
        {
            // https://kelseyc18.github.io/kademlia_vis/basics/3/
            // https://docs.rs/kademlia_routing_table/latest/kademlia_routing_table/
            var peerId = PeerId.Build("localhost", ROOT_NODE_PORT).Serialize();
            _peers = new List<(IPeerHost, IServiceProvider)>();
            var rootPeer = PeerHostFactory.NewStructured(o =>
            {
                o.Url = "localhost";
                o.Port = ROOT_NODE_PORT;
            })
                .UseUDPTransport()
                .AddDHTKademliaProtocol(o =>
                {
                    o.SeedPort = ROOT_NODE_PORT;
                    o.SeedUrl = "localhost";
                    o.IsSeedPeer = true;
                    o.KademliaPeerId = ROOT_PEER_ID;
                })
                .BuildWithDI();
            _peers.Add(rootPeer);
            rootPeer.Item1.Start().Wait();
            var line = string.Empty;
            do
            {
                Console.WriteLine("add-peer: Add peer");
                Console.WriteLine("add-key: Add key");
                Console.WriteLine("get-key: Get key");
                Console.WriteLine("stop-peer: Stop peer");
                Console.WriteLine("kbucket : Display kbucket");
                Console.WriteLine("data: Display data");
                Console.WriteLine("q: Exit");
                line = Console.ReadLine();
                if(line == "add-peer") AddPeer();
                if(line == "add-key") AddKey();
                if (line == "stop-peer") StopPeer();
                if (line == "get-key") GetKey();
                if(line == "kbucket") DisplayKBucketLst();
                if (line == "data") DisplayData();
            }
            while (line != "q");

            Console.WriteLine("Press enter to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static async void AddPeer()
        {
            Console.WriteLine("Enter a peer identifier");
            var peerId = long.Parse(Console.ReadLine());
            var peer = PeerHostFactory.NewStructured(o =>
            {
                o.Url = "localhost";
                o.Port = CURRENT_NODE_PORT;
            })
                .UseUDPTransport()
                .AddDHTKademliaProtocol(o =>
                {
                    o.SeedPort = ROOT_NODE_PORT;
                    o.SeedUrl = "localhost";
                    o.IsSeedPeer = false;
                    o.KademliaPeerId = peerId;
                })
                .BuildWithDI();
            await peer.Item1.Start();
            _peers.Add(peer);
            CURRENT_NODE_PORT++;
        }

        private static async void AddKey()
        {
            Console.WriteLine("Enter a key");
            var key = long.Parse(Console.ReadLine());
            Console.WriteLine("Enter a value");
            var value = Console.ReadLine();
            using (var kademliaClient = new UDPKademliaClient("localhost", ROOT_NODE_PORT))
            {
                await kademliaClient.StoreValue(key, value);
            }
        }

        private static async void StopPeer()
        {
            Console.WriteLine("Enter the peer identifier");
            var id = int.Parse(Console.ReadLine());
            var peer = _peers.First(p =>
            {
                var peerInfo = p.Item2.GetService(typeof(IDHTPeerInfoStore)) as IDHTPeerInfoStore;
                return peerInfo.Get().Id == id;
            });
            peer.Item1.Stop();
            _peers.Remove(peer);
        }

        private static async void GetKey()
        {
            Console.WriteLine("Enter a key");
            var key = long.Parse(Console.ReadLine());
            using (var kademliaClient = new UDPKademliaClient("localhost", ROOT_NODE_PORT))
            {
                var value = await kademliaClient.FindValue(key);
                Console.WriteLine($"Key {key}, Value {value.Value}");
            }
        }

        private static void DisplayKBucketLst()
        {
            foreach(var rec in _peers)
            {
                var peerInfoStore = rec.Item2.GetService(typeof(IDHTPeerInfoStore)) as IDHTPeerInfoStore;
                var peerInfo = peerInfoStore.Get();
                Console.WriteLine($"======= Peer identifier {peerInfo.Id}, Port {peerInfo.Port} =======");
                for(var i = 0; i < peerInfo.KBucketLst.Count; i++)
                {
                    var kbucket = peerInfo.KBucketLst.ElementAt(i);
                    Console.WriteLine($"=== Bucket {i}, Start {kbucket.Start}, End {kbucket.End} ===");
                    foreach (var kbucketPeer in kbucket.Peers) Console.WriteLine($"Peer identifier {kbucketPeer.PeerId}, Port {kbucketPeer.Port}, Is disabled {kbucketPeer.IsDisabled}");
                }

                Console.WriteLine();
            }
        }

        private static void DisplayData()
        {
            foreach(var rec in _peers)
            {
                var peerInfoStore = rec.Item2.GetService(typeof(IDHTPeerInfoStore)) as IDHTPeerInfoStore;
                var peerDataStore = rec.Item2.GetService(typeof(IPeerDataStore)) as IPeerDataStore;
                var peerInfo = peerInfoStore.Get();
                Console.WriteLine($"======= Peer identifier {peerInfo.Id}, Port {peerInfo.Port} =======");
                var allData = peerDataStore.GetAll();
                foreach(var data in allData)
                {
                    Console.WriteLine($"Identifier {data.Id}, Value {data.Value}");
                }

                Console.WriteLine();
            }
        }
    }
}
