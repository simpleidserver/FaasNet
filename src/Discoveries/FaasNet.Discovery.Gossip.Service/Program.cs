﻿using FaasNet.Discovery.Gossip.Client;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;

namespace FaasNet.Discovery.Gossip.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var seedPeerHostpeerHost = LaunchGossipPeer(new ConcurrentBag<ClusterPeer>(), 5001, "seedId");
            var secondPeerHostpeerHost = LaunchGossipPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, 5002, "peerId2");
            var thirdPeerHostpeerHost = LaunchGossipPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, 5003, "peerId3");
            Console.WriteLine("Press any key to display all the Peers present in the network");
            Console.ReadLine();
            DisplayCluster();
            Console.WriteLine("Press any key to stop the servers");
            Console.ReadLine();
            seedPeerHostpeerHost.Stop();
            secondPeerHostpeerHost.Stop();
            thirdPeerHostpeerHost.Stop();
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static IPeerHost LaunchGossipPeer(ConcurrentBag<ClusterPeer> clusterPeers, int port = 5001, string peerId = "peerId")
        {
            var peerHost = PeerHostFactory.New(o => {
                o.Url = "localhost";
                o.Port = port;
                o.PeerId = peerId;
            }, clusterPeers)
                .UseUDPTransport()
                .UseGossipDiscovery()
                .Build();
            peerHost.Start();
            return peerHost;
        }

        private static void DisplayCluster()
        {
            using (var gossipClient = new UDPGossipClient("localhost", 5002))
            {
                var peerInfos = gossipClient.Get().Result;
                foreach(var peerInfo in peerInfos)
                {
                    Console.WriteLine($"Url = {peerInfo.Url}, Port = {peerInfo.Port}");
                }
            }
        }
    }
}