using FaasNet.Partition.Client.Messages;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Transports;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public class PartitionedNodeHost : BasePeerHost
    {
        private readonly IPartitionCluster _partitionCluster;

        public PartitionedNodeHost(IPartitionCluster partitionCluster, ITransport transport, IProtocolHandlerFactory protocolHandlerFactory, IEnumerable<ITimer> timers, ILogger<BasePeerHost> logger) : base(transport, protocolHandlerFactory, timers, logger)
        {
            _partitionCluster = partitionCluster;
        }

        protected override async Task Init(CancellationToken cancellationToken = default)
        {
            await _partitionCluster.Start();
        }

        protected override async Task ReceiveMessage(BaseSessionResult session)
        {
            var payload = await session.ReceiveMessage();
            if (payload.Length == 0) return;
            var readBufferContext = new ReadBufferContext(payload);
            var partitionedRequest = BasePartitionedRequest.Deserialize(readBufferContext);
            byte[] result = null;
            if (partitionedRequest.Command == PartitionedCommands.TRANSFERED_REQUEST) result = await Handle(partitionedRequest as TransferedRequest);
            await session.SendMessage(result);
        }

        private Task<byte[]> Handle(TransferedRequest request)
        {
            return _partitionCluster.Transfer(request, TokenSource.Token);
        }
    }
}
