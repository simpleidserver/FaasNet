using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.RaftConsensus.Tests
{
    public class PeerHostFixture
    {
        [Fact]
        public async Task When_Launch_Two_Nodes()
        {
            // ARRANGE
            var peers = new ConcurrentBag<PeerInfo>
            {
                new PeerInfo { TermId = "termId", TermIndex = 0 }
            };
            var clusterNodes = new ConcurrentBag<ClusterNode>
            {
                new ClusterNode
                {
                    Port = 4001,
                    Url = "localhost"
                },
                new ClusterNode
                {
                    Port = 4002,
                    Url = "localhost"
                }
            };
            var firstNode = BuildNodeHost(peers, 4001, clusterNodes);
            var secondNode = BuildNodeHost(peers, 4002, clusterNodes);

            // ACT
            await firstNode.Start(CancellationToken.None);
            await secondNode.Start(CancellationToken.None);

            Thread.Sleep(200000);
        }

        private static INodeHost BuildNodeHost(ConcurrentBag<PeerInfo> peers, int port, ConcurrentBag<ClusterNode> clusterNodes)
        {
            var serviceProvider = new ServiceCollection()
                .AddConsensusPeer(o => o.Port = port)
                .SetPeers(peers)
                .SetClusterNodes(clusterNodes)
                .Services
                .BuildServiceProvider();
            return serviceProvider.GetService<INodeHost>();
        }
    }
}
