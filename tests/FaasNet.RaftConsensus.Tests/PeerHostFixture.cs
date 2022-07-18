using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.Stores;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.RaftConsensus.Tests
{
    public class PeerHostFixture
    {
        #region Consensus

        [Fact]
        public async Task When_AppendLogInOnePartition_Then_LogIsReplicated()
        {
            // ARRANGE
            var firstPeer = BuildPeer(4001, new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 4002) }, new ConcurrentBag<PartitionElectionRecord> { PartitionElectionRecord.Build("partition") });
            var secondPeer = BuildPeer(4002, new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 4001) }, new ConcurrentBag<PartitionElectionRecord> { PartitionElectionRecord.Build("partition") });
            await firstPeer.Start();
            await secondPeer.Start();
            // ACT
            using (var raftConsensusClient = new UDPRaftConsensusClient("localhost", 4002))
            {
                await raftConsensusClient.AppendEntry("partition", "value", CancellationToken.None);
            }

            Thread.Sleep(3000000);
            string ss = "";
        }

        /*
        [Fact]
        public async Task When_AppendLogInTwoPartitions_Then_LogIsReplicated()
        {
            // ARRANGE
            const int expectedNumberOfNodes = 2;
            var clusterStore = new InMemoryClusterStore();
            var firstNodeResult = BuildNodeHost(new ConcurrentBag<PartitionInfo> { new PartitionInfo { TermId = "termId", TermIndex = 0 }, new PartitionInfo { TermId = "secondTermId", TermIndex = 0 } }, 4008, clusterStore);
            var secondNodeResult = BuildNodeHost(new ConcurrentBag<PartitionInfo> { new PartitionInfo { TermId = "termId", TermIndex = 0 }, new PartitionInfo { TermId = "secondTermId", TermIndex = 0 } }, 4009, clusterStore);
            await firstNodeResult.NodeHost.Start(CancellationToken.None);
            await secondNodeResult.NodeHost.Start(CancellationToken.None);
            WaitNodeIsStarted(firstNodeResult.NodeHost);
            WaitNodeIsStarted(secondNodeResult.NodeHost);
            while ((await clusterStore.GetAllNodes(CancellationToken.None)).Count() != expectedNumberOfNodes) Thread.Sleep(500);
            var allNodes = new List<INodeHost> { firstNodeResult.NodeHost, secondNodeResult.NodeHost };

            // ACT
            WaitOnlyOneLeader(allNodes, "termId");
            WaitOnlyOneLeader(allNodes, "secondTermId");
            var client = new RaftConsensusClient("localhost", 4008);
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
        */

        #endregion

        private static IPeerHost BuildPeer(int port, ConcurrentBag<ClusterPeer> peers, ConcurrentBag<PartitionElectionRecord> partitionElectionRecords)
        {
            var peerHost = PeerHostFactory.New(o =>
            {
                o.Port = port;
                o.Url = "localhost";
            }, peers)
                .AddRaftConsensus(partitionElectionRecords: partitionElectionRecords)
                .UseUDPTransport()
                .Build();
            return peerHost;
        }
    }
}
