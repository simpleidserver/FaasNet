using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
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
            const string partitionName = "partition";
            var firstPeer = BuildPeer(4001, new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 4002) }, new ConcurrentBag<PartitionElectionRecord> { PartitionElectionRecord.Build(partitionName) });
            var secondPeer = BuildPeer(4002, new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 4001) }, new ConcurrentBag<PartitionElectionRecord> { PartitionElectionRecord.Build(partitionName) });
            await firstPeer.Start();
            await secondPeer.Start();

            // ACT
            using (var raftConsensusClient = new UDPRaftConsensusClient("localhost", 4002))
            {
                await raftConsensusClient.AppendEntry(partitionName, "value", CancellationToken.None);
            }

            var firstPeerEntry = WaitLogEntry("localhost", 4001, partitionName);
            var secondPeerEntry = WaitLogEntry("localhost", 4002, partitionName);

            // ASSERT
            Assert.NotNull(firstPeerEntry);
            Assert.NotNull(secondPeerEntry);
            Assert.Equal("value", firstPeerEntry.Value);
            Assert.Equal("value", secondPeerEntry.Value);
        }

        [Fact]
        public async Task When_AppendLogInTwoPartitions_Then_LogIsReplicated()
        {
            // ARRANGE
            const string firstPartitionName = "firstPartition";
            const string secondPartitionName = "secondPartition";
            var firstPeer = BuildPeer(4003, new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 4004) }, new ConcurrentBag<PartitionElectionRecord> { PartitionElectionRecord.Build(firstPartitionName), PartitionElectionRecord.Build(secondPartitionName) });
            var secondPeer = BuildPeer(4004, new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 4003) }, new ConcurrentBag<PartitionElectionRecord> { PartitionElectionRecord.Build(firstPartitionName), PartitionElectionRecord.Build(secondPartitionName) });
            await firstPeer.Start();
            await secondPeer.Start();

            // ACT
            using (var raftConsensusClient = new UDPRaftConsensusClient("localhost", 4003))
            {
                await raftConsensusClient.AppendEntry(firstPartitionName, "firstValue", CancellationToken.None);
                await raftConsensusClient.AppendEntry(secondPartitionName, "secondValue", CancellationToken.None);
            }

            var firstPeerFirstPartitionEntry = WaitLogEntry("localhost", 4003, firstPartitionName);
            var secondPeerFirstPartitionEntry = WaitLogEntry("localhost", 4004, firstPartitionName);
            var firstPeerSecondPartitionEntry = WaitLogEntry("localhost", 4003, secondPartitionName);
            var secondPeerSecondPartitionEntry = WaitLogEntry("localhost", 4004, secondPartitionName);

            // ASSERT
            Assert.NotNull(firstPeerFirstPartitionEntry);
            Assert.NotNull(secondPeerFirstPartitionEntry);
            Assert.NotNull(firstPeerSecondPartitionEntry);
            Assert.NotNull(secondPeerSecondPartitionEntry);
        }

        #endregion

        private static IPeerHost BuildPeer(int port, ConcurrentBag<ClusterPeer> peers, ConcurrentBag<PartitionElectionRecord> partitionElectionRecords)
        {
            var peerHost = PeerHostFactory.NewUnstructured(o =>
            {
                o.Port = port;
                o.Url = "localhost";
            }, peers)
                .AddRaftConsensus(partitionElectionRecords: partitionElectionRecords)
                .UseUDPTransport()
                .Build();
            return peerHost;
        }

        private static GetEntryResult WaitLogEntry(string url, int port, string replicationId)
        {
            using(var raftConsensusClient = new UDPRaftConsensusClient(url, port))
            {
                var entry = raftConsensusClient.GetEntry(replicationId).Result;
                if(entry.IsNotFound)
                {
                    Thread.Sleep(100);
                    return WaitLogEntry(url, port, replicationId);
                }

                return entry;
            }
        }
    }
}
