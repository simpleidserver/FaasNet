using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.RaftConsensus.Tests
{
    public class PeerHostFixture
    {
        #region Consensus

        [Fact]
        public async Task When_AppendLog_Then_LogIsReplicated()
        {
            // ARRANGE
            var leaderSem = new SemaphoreSlim(0);
            var firstPeer = BuildPeer(4001, new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 4002) }, leaderSem, "node1");
            var secondPeer = BuildPeer(4002, new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 4001) }, leaderSem, "node2");
            await firstPeer.Start();
            await secondPeer.Start();
            leaderSem.Wait();
            var cmd = Encoding.UTF8.GetBytes("value");

            // ACT
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", 4002, ClientTransportFactory.NewUDP()))
            {
                await client.AppendEntry("stateMachineId", cmd);
            }

            var firstPeerEntry = await WaitLogEntries("localhost", 4001, 1);
            var secondPeerEntry = await WaitLogEntries("localhost", 4002, 1);

            // ASSERT
            Assert.NotNull(firstPeerEntry);
            Assert.NotNull(secondPeerEntry);
            Assert.Equal(1, firstPeerEntry.Entries.First().Index);
            Assert.Equal(1, secondPeerEntry.Entries.First().Index);
        }

        #endregion

        private static IPeerHost BuildPeer(int port, ConcurrentBag<ClusterPeer> peers, SemaphoreSlim sem, string nodeName = "node1")
        {
            var path = Path.GetTempPath();
            var peerHost = PeerHostFactory.NewUnstructured(o =>
            {
                o.Port = port;
                o.Url = "localhost";
            }, peers)
                .UseUDPTransport()
                .AddRaftConsensus(o =>
                {
                    o.ConfigurationDirectoryPath = Path.Combine(path, nodeName);
                    o.LeaderCallback = () =>
                    {
                        sem.Release();
                    };
                })
                .Build();
            return peerHost;
        }

        private static async Task<GetLogsResult> WaitLogEntries(string url, int port, int startIndex)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, ClientTransportFactory.NewUDP()))
            {
                var entry = (await client.GetLogs(startIndex)).First();
                if (entry.Entries.Any()) return entry;
            }

            Thread.Sleep(100);
            return await WaitLogEntries(url, port, startIndex);
        }
    }
}
