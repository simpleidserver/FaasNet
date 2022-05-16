using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public class StandalonePeerHost : BasePeerHost
    {
        public StandalonePeerHost(ILogger<BasePeerHost> logger, IOptions<ConsensusPeerOptions> options, IClusterStore clusterStore, ILogStore logStore, IPeerInfoStore peerStore) : base(logger, options, clusterStore, logStore, peerStore)
        {
        }

        protected override Task AddEntry(LogRecord logRecord, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task<bool> HandlePackage(UdpReceiveResult udpResult)
        {
            return Task.FromResult(true);
        }

        protected override Task Init(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
