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
        public StandaloneNodeHost(IPeerStore peerStore, IPeerHostFactory peerHostFactory, ILogger<BaseNodeHost> logger, IOptions<ConsensusPeerOptions> options) : base(peerStore, peerHostFactory, logger, options)
        {
        }

        protected override Task HandleUDPPackage(UdpReceiveResult udpResult, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
