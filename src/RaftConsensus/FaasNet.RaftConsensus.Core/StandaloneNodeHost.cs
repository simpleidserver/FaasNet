using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FaasNet.RaftConsensus.Core
{
    public class StandaloneNodeHost : BaseNodeHost
    {
        public StandaloneNodeHost(IPeerStore peerStore, IPeerHostFactory peerHostFactory, ILogger<BaseNodeHost> logger, IOptions<ConsensusPeerOptions> options) : base(peerStore, peerHostFactory, logger, options)
        {
        }
    }
}
