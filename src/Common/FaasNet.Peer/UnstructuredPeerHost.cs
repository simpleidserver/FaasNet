using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer
{
    public class UnstructuredPeerHost : BasePeerHost
    {
        private readonly IClusterStore _clusterStore;
        private readonly PeerOptions _options;

        public UnstructuredPeerHost(IClusterStore clusterStore, IOptions<PeerOptions> options, ITransport transport, IProtocolHandlerFactory protocolHandlerFactory, IEnumerable<ITimer> timers, ILogger<BasePeerHost> logger) : base(transport, protocolHandlerFactory, timers, logger)
        {
            _clusterStore = clusterStore;
            _options = options.Value;
        }

        protected override Task Init(CancellationToken cancellationToken = default)
        {
            if (!_options.IsSelfRegistrationEnabled) return Task.CompletedTask;
            return _clusterStore.SelfRegister(new ClusterPeer(_options.Url, _options.Port) { PartitionKey = _options.PartitionKey }, cancellationToken);
        }
    }
}
