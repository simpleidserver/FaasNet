using FaasNet.Common;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages.Gossip;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
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
        public async Task When_LaunchTwoNodes_Then_ClusterIsFormed()
        {
            // ARRANGE
            const int expectedNumberOfNodes = 2;
            var clusterStore = new InMemoryClusterStore();
            var firstNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4000, clusterStore);
            var secondNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4001, clusterStore);
            await firstNodeResult.NodeHost.Start(CancellationToken.None);
            await secondNodeResult.NodeHost.Start(CancellationToken.None);
            WaitNodeIsStarted(firstNodeResult.NodeHost);
            WaitNodeIsStarted(secondNodeResult.NodeHost);

            // ACT
            while ((await clusterStore.GetAllNodes(CancellationToken.None)).Count() != expectedNumberOfNodes) Thread.Sleep(500);
            ICollection<ClusterNodeResult> clusterNodes;
            using (var gossipClient = new GossipClient("localhost", 4000)) clusterNodes = await gossipClient.GetClusterNodes();
            await firstNodeResult.NodeHost.Stop();
            await secondNodeResult.NodeHost.Stop();
            var nodeClusterNodes = await clusterStore.GetAllNodes(CancellationToken.None);

            // ASSERT
            Assert.Equal(2, nodeClusterNodes.Count());
            Assert.True(nodeClusterNodes.Any(cn => cn.Port == 4000 && cn.Url == "localhost") == true);
            Assert.True(nodeClusterNodes.Any(cn => cn.Port == 4001 && cn.Url == "localhost") == true);
        }

        [Fact]
        public async Task When_OneNodeIsStopped_Then_TheNodeBecomeUnreachable()
        {
            // ARRANGE
            const int expectedNumberOfNodes = 2;
            var clusterStore = new InMemoryClusterStore();
            var firstNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4002, clusterStore);
            var secondNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4003, clusterStore);
            await firstNodeResult.NodeHost.Start(CancellationToken.None);
            await secondNodeResult.NodeHost.Start(CancellationToken.None);
            WaitNodeIsStarted(firstNodeResult.NodeHost);
            WaitNodeIsStarted(secondNodeResult.NodeHost);
            while ((await clusterStore.GetAllNodes(CancellationToken.None)).Count() != expectedNumberOfNodes) Thread.Sleep(500);

            // ACT
            await secondNodeResult.NodeHost.Stop();
            var clusterNodes = WaitUnreachableClusterNodes(firstNodeResult.NodeHost, (un) => un.Count() == 1);
            await firstNodeResult.NodeHost.Stop();

            // ASSERT
            Assert.Single(clusterNodes);
            Assert.Equal("localhost", clusterNodes.First().Node.Url);
            Assert.Equal(4003, clusterNodes.First().Node.Port);
        }

        [Fact]
        public async Task When_OneNodeStateIsAdded_Then_StorageIsUpdated()
        {
            // ARRANGE
            const int expectedNumberOfNodes = 2;
            var clusterStore = new InMemoryClusterStore();
            var firstNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4004, clusterStore);
            var secondNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo>(), 4005, clusterStore);
            await firstNodeResult.NodeHost.Start(CancellationToken.None);
            await secondNodeResult.NodeHost.Start(CancellationToken.None);
            WaitNodeIsStarted(firstNodeResult.NodeHost);
            WaitNodeIsStarted(secondNodeResult.NodeHost);
            while ((await clusterStore.GetAllNodes(CancellationToken.None)).Count() != expectedNumberOfNodes) Thread.Sleep(500);

            // ACT
            using (var gossipClient = new GossipClient("localhost", 4004)) await gossipClient.UpdateNodeState("Client", "id", "value");
            var seedClient = (await WaitEntityTypes(firstNodeResult.NodeHost, (nodes) => nodes.Any(n => n.EntityType == "Client"))).First(c => c.EntityType == "Client");
            var firstNodeClient = (await WaitEntityTypes(secondNodeResult.NodeHost, (nodes) => nodes.Any(n => n.EntityType == "Client"))).First(c => c.EntityType == "Client");

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
            const int expectedNumberOfNodes = 2;
            var clusterStore = new InMemoryClusterStore();
            var firstNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo> { new PeerInfo { TermId = "termId", TermIndex = 0 } }, 4006, clusterStore);
            var secondNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo> { new PeerInfo { TermId = "termId", TermIndex = 0 } }, 4007, clusterStore);
            await firstNodeResult.NodeHost.Start(CancellationToken.None);
            await secondNodeResult.NodeHost.Start(CancellationToken.None);
            WaitNodeIsStarted(firstNodeResult.NodeHost);
            WaitNodeIsStarted(secondNodeResult.NodeHost);
            while ((await clusterStore.GetAllNodes(CancellationToken.None)).Count() != expectedNumberOfNodes) Thread.Sleep(500);
            var allNodes = new List<INodeHost> { firstNodeResult.NodeHost, secondNodeResult.NodeHost };

            // ACT
            WaitOnlyOneLeader(allNodes, "termId");
            var client = new ConsensusClient("localhost", 4006);
            client.AppendEntry("termId", "value", CancellationToken.None).Wait();
            await WaitLogs(allNodes, 1, p => p.Info.TermId == "termId", l => l.Value == "value" && l.Index == 1);

            // ASSERT
            var firstNodeLogRecord = await firstNodeResult.NodeHost.Peers.First().ReadRecord(1, CancellationToken.None);
            var secondNodeLogRecord = await secondNodeResult.NodeHost.Peers.First().ReadRecord(1, CancellationToken.None);
            Assert.Equal("value", firstNodeLogRecord.Value);
            Assert.Equal("value", secondNodeLogRecord.Value);
            await firstNodeResult.NodeHost.Stop();
            await secondNodeResult.NodeHost.Stop();
        }

        [Fact]
        public async Task When_AppendLogInTwoPartitions_Then_LogIsReplicated()
        {
            // ARRANGE
            const int expectedNumberOfNodes = 2;
            var clusterStore = new InMemoryClusterStore();
            var firstNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo> { new PeerInfo { TermId = "termId", TermIndex = 0 }, new PeerInfo { TermId = "secondTermId", TermIndex = 0 } }, 4008, clusterStore);
            var secondNodeResult = BuildNodeHost(new ConcurrentBag<PeerInfo> { new PeerInfo { TermId = "termId", TermIndex = 0 }, new PeerInfo { TermId = "secondTermId", TermIndex = 0 } }, 4009, clusterStore);
            await firstNodeResult.NodeHost.Start(CancellationToken.None);
            await secondNodeResult.NodeHost.Start(CancellationToken.None);
            WaitNodeIsStarted(firstNodeResult.NodeHost);
            WaitNodeIsStarted(secondNodeResult.NodeHost);
            while ((await clusterStore.GetAllNodes(CancellationToken.None)).Count() != expectedNumberOfNodes) Thread.Sleep(500);
            var allNodes = new List<INodeHost> { firstNodeResult.NodeHost, secondNodeResult.NodeHost };

            // ACT
            WaitOnlyOneLeader(allNodes, "termId");
            WaitOnlyOneLeader(allNodes, "secondTermId");
            var client = new ConsensusClient("localhost", 4008);
            client.AppendEntry("termId", "value", CancellationToken.None).Wait();
            client.AppendEntry("secondTermId", "value", CancellationToken.None).Wait();
            await WaitLogs(allNodes, 1, p => p.Info.TermId == "termId", l => l.Value == "value" && l.Index == 1);
            await WaitLogs(allNodes, 1, p => p.Info.TermId == "secondTermId", l => l.Value == "value" && l.Index == 1);

            // ASSERT
            var firstNodeTermLogRecord = await firstNodeResult.NodeHost.Peers.First(p => p.Info.TermId == "termId").ReadRecord(1, CancellationToken.None);
            var firstNodeSecondTermLogRecord = await firstNodeResult.NodeHost.Peers.First(p => p.Info.TermId == "secondTermId").ReadRecord(1, CancellationToken.None);
            var secondNodeTermLogRecord = await secondNodeResult.NodeHost.Peers.First(p => p.Info.TermId == "termId").ReadRecord(1, CancellationToken.None);
            var secondNodeSecondTermLogRecord = await secondNodeResult.NodeHost.Peers.First(p => p.Info.TermId == "termId").ReadRecord(1, CancellationToken.None);
            Assert.Equal("value", firstNodeTermLogRecord.Value);
            Assert.Equal("value", firstNodeSecondTermLogRecord.Value);
            Assert.Equal("value", secondNodeTermLogRecord.Value);
            Assert.Equal("value", secondNodeSecondTermLogRecord.Value);
            await firstNodeResult.NodeHost.Stop();
            await secondNodeResult.NodeHost.Stop();
        }

        #endregion

        private static NodeResult BuildNodeHost(ConcurrentBag<PeerInfo> peers, int port, InMemoryClusterStore clusterStore)
        {
            var serverBuilder = new ServerBuilder()
                .AddConsensusPeer(o =>
                {
                    o.Port = port;
                    o.ExposedPort = port;
                })
                .SetPeers(peers);
            var type = serverBuilder.Services.First(s => s.ServiceType == typeof(IClusterStore));
            serverBuilder.Services.Remove(type);
            serverBuilder.Services.AddSingleton<IClusterStore>(clusterStore);
            var serviceProvider = serverBuilder.ServiceProvider;
            var nodeHost = serviceProvider.GetRequiredService<INodeHost>();
            return new NodeResult { NodeHost = nodeHost };
        }

        private class NodeResult
        {
            public INodeHost NodeHost { get; set; }
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
                var entityTypes = await node.NodeStateStore.GetAllLastEntityTypes(CancellationToken.None);
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

        private static async Task WaitLogs(List<INodeHost> nodes, int evtOffset, Func<IPeerHost, bool> callbackPeers, Func<LogRecord, bool> callbackLogRecords)
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
                        var log = await filteredPeer.ReadRecord(evtOffset, CancellationToken.None);
                        if (log == null || !callbackLogRecords(log)) isLogPropagated = false;
                    }
                }

                Thread.Sleep(200);
            }
        }
    }
}
