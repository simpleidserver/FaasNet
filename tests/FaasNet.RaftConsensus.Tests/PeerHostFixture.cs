using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.RaftConsensus.Tests
{
    public class PeerHostFixture
    {
        [Fact]
        public async Task When_Launch_Follower()
        {
            // ARRANGE
            var serviceProvider = new ServiceCollection().AddConsensusPeer().BuildServiceProvider();
            var peerHostFactory = serviceProvider.GetService<IPeerHostFactory>();
            var peerHost = peerHostFactory.Build();
            await peerHost.Start(new Core.Models.PeerInfo { TermId = "termId", TermIndex = 0 }, CancellationToken.None);

            // ACT
            using (var consensusClient = new ConsensusClient(peerHost.UdpServerEdp))
            {
                await consensusClient.LeaderHeartbeat("termId", 0, CancellationToken.None);
            }

            string sss = "";
            // ASSERT
        }
    }
}
