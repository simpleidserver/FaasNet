using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;

namespace FaasNet.RaftConsensus.Service
{
    internal partial class Program
    {
        public static void LaunchMenu()
        {
            LaunchRaftConsensusPeerGCollectionStr(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, false, "node1", 5001);
            LaunchRaftConsensusPeerGCollectionStr(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, false, "node2", 5002);

            bool exit = false;
            do
            {
                Console.WriteLine("===== MENU =====");
                Console.WriteLine("add: Add a string into the collection");
                Console.WriteLine("remove: Remove a string from the collection");
                Console.WriteLine("statemachine: Display state machine");
                Console.WriteLine("state: Display peer state");
                Console.WriteLine("exit");
                var menu = Console.ReadLine();
                switch(menu)
                {
                    case "add":
                        AddStr();
                        break;
                    case "remove":
                        RemoveStr();
                        break;
                    case "statemachine":
                        DisplayGCollectionStateMachine();
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

        private static void AddStr()
        {
            Console.WriteLine("Enter the port");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter a string");
            var str = Console.ReadLine();
            AddStr(port, str, false);
        }

        private static void RemoveStr()
        {
            Console.WriteLine("Enter the port");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine("Remove a string");
            var str = Console.ReadLine();
            RemoveStr(port, str, false);
        }

        private static void DisplayGCollectionStateMachine()
        {
            Console.WriteLine("Enter the port");
            int port = int.Parse(Console.ReadLine());
            DisplayGCollectionStateMachine(port, false);
        }

        private static void DisplayPeerState()
        {
            Console.WriteLine("Enter the port");
            int port = int.Parse(Console.ReadLine());
            DisplayPeerState(port, false);
        }
    }
}
