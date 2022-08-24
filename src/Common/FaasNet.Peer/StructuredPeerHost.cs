using FaasNet.Peer.Transports;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer
{
    public class StructuredPeerHost : BasePeerHost
    {
        public StructuredPeerHost(IServerTransport transport, IProtocolHandlerFactory protocolHandlerFactory, IEnumerable<ITimer> timers, ILogger<BasePeerHost> logger) : base(transport, protocolHandlerFactory, timers, logger)
        {
        }

        protected override Task Init(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
