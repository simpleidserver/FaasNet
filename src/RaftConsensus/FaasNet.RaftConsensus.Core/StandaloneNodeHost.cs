using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public class StandaloneNodeHost : BaseNodeHost
    {
        public StandaloneNodeHost(IPeerStore peerStore, IPeerInfoStore peerInfoStore, IPeerHostFactory peerHostFactory, INodeStateStore nodeStateStore, IClusterStore clusterStore, ILogger<BaseNodeHost> logger, IOptions<ConsensusNodeOptions> options) : base(peerStore, peerInfoStore, peerHostFactory, nodeStateStore, clusterStore, logger, options)
        {
        }

        protected override Task HandlePackage(UdpReceiveResult udpResult, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
