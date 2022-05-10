using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.RaftConsensus.Tests
{
    public class PeerHostFixture
    {
        #region Gossip

        [Fact]
        public async Task When_JoinNodeToCluster_Then_StorageIsUpdated()
        {
            // ARRANGE
            var seedNode = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4000, new ConcurrentBag<ClusterNode>(), true);
            var firstNode = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4001, new ConcurrentBag<ClusterNode>());
            await seedNode.Start(CancellationToken.None);
            await firstNode.Start(CancellationToken.None);
            WaitNodeIsStarted(seedNode);
            WaitNodeIsStarted(firstNode);

            // ACT
            using (var gossipClient = new GossipClient("localhost", 4000)) await gossipClient.JoinNode("localhost", 4001);
            var seedNodeStates = await WaitEntityTypes(seedNode, (nodes) => nodes.Count() == 2);
            var firstNodeStates = await WaitEntityTypes(firstNode, (nodes) => nodes.Count() == 2);
            await seedNode.Stop();
            await firstNode.Stop();

            // ASSERT
            Assert.Equal(2, seedNodeStates.Count());
            Assert.Equal(2, firstNodeStates.Count());
            Assert.Equal(StandardEntityTypes.Cluster, seedNodeStates.ElementAt(0).EntityType);
            Assert.Equal(StandardEntityTypes.Cluster, seedNodeStates.ElementAt(1).EntityType);
            Assert.Equal("{\"Url\":\"localhost\",\"Port\":4000}", seedNodeStates.ElementAt(1).Value);
            Assert.Equal("{\"Url\":\"localhost\",\"Port\":4001}", seedNodeStates.ElementAt(0).Value);
            Assert.Equal("{\"Url\":\"localhost\",\"Port\":4000}", firstNodeStates.ElementAt(1).Value);
            Assert.Equal("{\"Url\":\"localhost\",\"Port\":4001}", firstNodeStates.ElementAt(0).Value);
        }

        [Fact]
        public async Task When_NodeIsStopped_Then_NodeBecomeUnreachable()
        {
            // ARRANGE
            var seedNode = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4001, new ConcurrentBag<ClusterNode>(), true);
            var firstNode = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4002, new ConcurrentBag<ClusterNode>());
            await seedNode.Start(CancellationToken.None);
            await firstNode.Start(CancellationToken.None);
            WaitNodeIsStarted(seedNode);
            WaitNodeIsStarted(firstNode);
            using (var gossipClient = new GossipClient("localhost", 4001)) await gossipClient.JoinNode("localhost", 4002);
            var seedNodeStates = await WaitEntityTypes(seedNode, (nodes) => nodes.Count() == 2);
            var firstNodeStates = await WaitEntityTypes(firstNode, (nodes) => nodes.Count() == 2);

            // ACT
            await firstNode.Stop();
            var clusterNodes = WaitUnreachableClusterNodes(seedNode, (un) => un.Count() == 1);
            await seedNode.Stop();

            // ASSERT
            Assert.Single(clusterNodes);
            Assert.Equal("localhost", clusterNodes.First().Node.Url);
            Assert.Equal(4002, clusterNodes.First().Node.Port);
        }

        [Fact]
        public async Task When_StateIsAdded_Then_StorageIsUpdated()
        {
            // ARRANGE
            var seedNode = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4003, new ConcurrentBag<ClusterNode>(), true);
            var firstNode = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4004, new ConcurrentBag<ClusterNode>());
            await seedNode.Start(CancellationToken.None);
            await firstNode.Start(CancellationToken.None);
            WaitNodeIsStarted(seedNode);
            WaitNodeIsStarted(firstNode);
            using (var gossipClient = new GossipClient("localhost", 4003)) await gossipClient.JoinNode("localhost", 4004);
            var seedNodeStates = await WaitEntityTypes(seedNode, (nodes) => nodes.Count() == 2);
            var firstNodeStates = await WaitEntityTypes(firstNode, (nodes) => nodes.Count() == 2);

            // ACT
            using (var gossipClient = new GossipClient("localhost", 4003)) await gossipClient.UpdateNodeState("Client", "id", "value");
            var seedClient = (await WaitEntityTypes(seedNode, (nodes) => nodes.Any(n => n.EntityType == "Client"))).First(c => c.EntityType == "Client");
            var firstNodeClient = (await WaitEntityTypes(firstNode, (nodes) => nodes.Any(n => n.EntityType == "Client"))).First(c => c.EntityType == "Client");

            // ASSERT
            Assert.NotNull(seedClient);
            Assert.NotNull(firstNodeClient);
            Assert.Equal("Client", seedClient.EntityType);
            Assert.Equal("Client", firstNodeClient.EntityType);
        }

        #endregion

        #region Consensus

        [Fact]
        public async Task When_AppendLogInOnePartition_Then_LogIsReplicated()
        {
            // ARRANGE
            var seedNode = BuildNodeHost(new ConcurrentBag<PeerInfo> { new PeerInfo { TermId = "termId", TermIndex = 0 } }, 4005, new ConcurrentBag<ClusterNode>(), true);
            var firstNode = BuildNodeHost(new ConcurrentBag<PeerInfo> { new PeerInfo { TermId = "termId", TermIndex = 0 } }, 4006, new ConcurrentBag<ClusterNode>());
            await seedNode.Start(CancellationToken.None);
            await firstNode.Start(CancellationToken.None);
            WaitNodeIsStarted(seedNode);
            WaitNodeIsStarted(firstNode);
            using (var gossipClient = new GossipClient("localhost", 4005)) await gossipClient.JoinNode("localhost", 4006);
            var seedNodeStates = await WaitEntityTypes(seedNode, (nodes) => nodes.Count() == 2);
            var firstNodeStates = await WaitEntityTypes(firstNode, (nodes) => nodes.Count() == 2);
            var allNodes = new List<INodeHost> { seedNode, firstNode };

            // ACT
            WaitOnlyOneLeader(allNodes, "termId");
            var client = new ConsensusClient("localhost", 4005);
            client.AppendEntry("termId", "value", CancellationToken.None).Wait();
            WaitLogs(allNodes, p => p.Info.TermId == "termId", l => l.Value == "value");

            // ASSERT
            var seedPeerLogs = seedNode.Peers.First().LogStore.GetAll(CancellationToken.None).Result;
            var firstPeerLogs = firstNode.Peers.First().LogStore.GetAll(CancellationToken.None).Result;
            Assert.Single(seedPeerLogs);
            Assert.Single(firstPeerLogs);
            Assert.Equal("value", seedPeerLogs.First().Value);
            Assert.Equal("value", firstPeerLogs.First().Value);
            await seedNode.Stop();
            await firstNode.Stop();
        }

        [Fact]
        public async Task When_AppendLogInTwoPartitions_Then_LogIsReplicated()
        {
            // ARRANGE
            var seedNode = BuildNodeHost(new ConcurrentBag<PeerInfo> { new PeerInfo { TermId = "termId", TermIndex = 0 }, new PeerInfo { TermId = "secondTermId", TermIndex = 0 } }, 4007, new ConcurrentBag<ClusterNode>(), true);
            var firstNode = BuildNodeHost(new ConcurrentBag<PeerInfo> { new PeerInfo { TermId = "termId", TermIndex = 0 }, new PeerInfo { TermId = "secondTermId", TermIndex = 0 } }, 4008, new ConcurrentBag<ClusterNode>());
            await seedNode.Start(CancellationToken.None);
            await firstNode.Start(CancellationToken.None);
            WaitNodeIsStarted(seedNode);
            WaitNodeIsStarted(firstNode);
            using (var gossipClient = new GossipClient("localhost", 4007)) await gossipClient.JoinNode("localhost", 4008);
            var seedNodeStates = await WaitEntityTypes(seedNode, (nodes) => nodes.Count() == 2);
            var firstNodeStates = await WaitEntityTypes(firstNode, (nodes) => nodes.Count() == 2);
            var allNodes = new List<INodeHost> { seedNode, firstNode };

            // ACT
            WaitOnlyOneLeader(allNodes, "termId");
            WaitOnlyOneLeader(allNodes, "secondTermId");
            var client = new ConsensusClient("localhost", 4007);
            client.AppendEntry("termId", "value", CancellationToken.None).Wait();
            client.AppendEntry("secondTermId", "value", CancellationToken.None).Wait();
            WaitLogs(allNodes, p => p.Info.TermId == "termId", l => l.Value == "value");
            WaitLogs(allNodes, p => p.Info.TermId == "secondTermId", l => l.Value == "value");

            // ASSERT
            var seedPeerLogs = seedNode.Peers.First().LogStore.GetAll(CancellationToken.None).Result;
            var firstPeerLogs = firstNode.Peers.First().LogStore.GetAll(CancellationToken.None).Result;
            Assert.Single(seedPeerLogs);
            Assert.Single(firstPeerLogs);
            Assert.Equal("value", seedPeerLogs.First().Value);
            Assert.Equal("value", firstPeerLogs.First().Value);
            await seedNode.Stop();
            await firstNode.Stop();
        }

        #endregion

        private static INodeHost BuildNodeHost(ConcurrentBag<PeerInfo> peers, int port, ConcurrentBag<ClusterNode> clusterNodes, bool isSeed = false)
        {
            var serviceCollection = new ServiceCollection()
                .AddConsensusPeer(o => o.Port = port);
            if (isSeed) serviceCollection.SetNodeStates(new ConcurrentBag<NodeState>(new List<NodeState> { new ClusterNode { Port = port, Url = "localhost" }.ToNodeState() }));
            var serviceProvider = serviceCollection.SetPeers(peers)
                // .SetNodeStates(new ConcurrentBag<NodeState>(clusterNodes.Select(n => n.ToNodeState())))
                .Services
                .BuildServiceProvider();
            return serviceProvider.GetService<INodeHost>();
        }

        private static void WaitNodeIsStarted(INodeHost node)
        {
            while(true)
            {
                if (node.IsStarted) return;
                Thread.Sleep(200);
            }
        }

        private static async Task<IEnumerable<NodeState>> WaitEntityTypes(INodeHost node, Func<IEnumerable<NodeState>, bool> callback)
        {
            while(true)
            {
                var entityTypes = await node.NodeStateStore.GetAllEntityTypes(CancellationToken.None);
                if (callback(entityTypes)) return entityTypes;
                Thread.Sleep(200);
            }
        }

        private static IEnumerable<UnreachableClusterNode> WaitUnreachableClusterNodes(INodeHost node, Func<IEnumerable<UnreachableClusterNode>, bool> callback)
        {
            while(true)
            {
                if (callback(node.UnreachableClusterNodes)) return node.UnreachableClusterNodes.ToArray();
                Thread.Sleep(200);
            }
        }

        private static void WaitOnlyOneLeader(List<INodeHost> nodes, string termId)
        {
            while (true)
            {
                var peers = nodes.SelectMany(n => n.Peers).Where(p => p.Info.TermId == termId);
                var leaderNodes = peers.Where(p => p.State == PeerStates.LEADER);
                var followerNodes = peers.Where(p => p.State == PeerStates.FOLLOWER && p.ActiveNode != null);
                if (leaderNodes.Count() == 1 && followerNodes.Count() == peers.Count() - 1)
                {
                    return;
                }

                Thread.Sleep(200);
            }
        }

        private static void WaitLogs(List<INodeHost> nodes, Func<IPeerHost, bool> callbackPeers, Func<LogRecord, bool> callbackLogRecords)
        {
            var isLogPropagated = false;
            while(!isLogPropagated)
            {
                isLogPropagated = true;
                foreach(var node in nodes)
                {
                    var filteredPeers = node.Peers.Where(callbackPeers);
                    foreach(var filteredPeer in filteredPeers)
                    {
                        var filteredLogs = filteredPeer.LogStore.GetAll(CancellationToken.None).Result.Where(callbackLogRecords);
                        if (!filteredLogs.Any()) isLogPropagated = false;
                    }
                }

                Thread.Sleep(200);
            }
        }
    }
}
