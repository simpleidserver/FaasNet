using FaasNet.Common.Extensions;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;

namespace FaasNet.RaftConsensus.Service
{
    internal partial class Program
    {
        private static ConcurrentBag<ClusterPeer> _clusterPeers = new ConcurrentBag<ClusterPeer>
        {
            new ClusterPeer("localhost", 5001),
            new ClusterPeer("localhost", 5002)
        };
        private static Dictionary<int, IPeerHost> _peers = new Dictionary<int, IPeerHost>();

        public static void LaunchMenu()
        {
            _peers.Add(5001, LaunchRaftConsensusPeerCounter(_clusterPeers, false, "node5001", 5001));
            _peers.Add(5002, LaunchRaftConsensusPeerCounter(_clusterPeers, false, "node5002", 5002));

            bool exit = false;
            do
            {
                Console.WriteLine("===== MENU =====");
                Console.WriteLine("increment: Increment a counter");
                Console.WriteLine("display: Display a counter");
                Console.WriteLine("addpeer: Add a peer");
                Console.WriteLine("stoppeer: Remove one peer");
                Console.WriteLine("peers: Display the peers");
                Console.WriteLine("state: Display peer state");
                Console.WriteLine("exit");
                var menu = Console.ReadLine();
                switch(menu)
                {
                    case "increment":
                        Increment();
                        break;
                    case "display":
                        Display();
                        break;
                    case "addpeer":
                        AddPeer();
                        break;
                    case "stoppeer":
                        StopPeer();
                        break;
                    case "peers":
                        DisplayPeers();
                        break;
                    case "state":
                        DisplayPeerState();
                        break;
                    case "exit":
                        exit = true;
                        break;
                }
            }
            while(!exit);
        }

        private static void Increment()
        {
            Console.WriteLine("Enter the port");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter a key");
            var key = Console.ReadLine();
            Console.WriteLine("Enter a value");
            var value = long.Parse(Console.ReadLine());
            IncrementCounter(key, port, value, false);
        }

        private static void Display()
        {
            Console.WriteLine("Enter the port");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter a key");
            var key = Console.ReadLine();
            DisplayCounter(key, port, false);
        }

        private static async void AddPeer()
        {
            Console.WriteLine("Enter the port");
            int port = int.Parse(Console.ReadLine());
            _clusterPeers.Add(new ClusterPeer("localhost", port));
            _peers.Add(port, LaunchRaftConsensusPeerCounter(_clusterPeers, false, $"node{port}", port));
        }

        private static async void StopPeer()
        {
            Console.WriteLine("Enter the port");
            int port = int.Parse(Console.ReadLine());
            await _peers[port].Stop();
            _peers.Remove(port);
            _clusterPeers.Remove(_clusterPeers.Single(p => p.Port == port));
        }

        private static void DisplayPeers()
        {
            foreach(var peer in _peers)
            {
                Console.WriteLine($"Peer {peer.Key}, Is running {peer.Value.IsRunning}");
            }
        }

        private static void DisplayPeerState()
        {
            Console.WriteLine("Enter the port");
            int port = int.Parse(Console.ReadLine());
            DisplayPeerState(port, false);
        }
    }
}
