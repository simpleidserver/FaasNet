using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public class PartitionedNodeHost : BasePeerHost
    {
        private readonly PeerOptions _peerOptions;
        private readonly IClusterStore _clusterStore;
        private readonly IPartitionCluster _partitionCluster;

        public PartitionedNodeHost(IOptions<PeerOptions> peerOptions, IClusterStore clusterStore, IPartitionCluster partitionCluster, IServerTransport transport, IProtocolHandlerFactory protocolHandlerFactory, IEnumerable<ITimer> timers, ILogger<BasePeerHost> logger) : base(transport, protocolHandlerFactory, timers, logger)
        {
            _peerOptions = peerOptions.Value;
            _clusterStore = clusterStore;
            _partitionCluster = partitionCluster;
        }

        protected IPartitionCluster PartitionCluster => _partitionCluster;
        protected IClusterStore ClusterStore => _clusterStore;
        protected PeerOptions PeerOptions => _peerOptions;

        protected override async Task Init(CancellationToken cancellationToken = default)
        {   
            await _clusterStore.SelfRegister(new ClusterPeer(_peerOptions.Url, _peerOptions.Port) { PartitionKey = "*" }, cancellationToken);
            await _partitionCluster.Start();
        }

        protected override async Task ReceiveMessage(BaseSessionResult session)
        {
            var payload = await session.ReceiveMessage();
            if (payload.Length == 0) return;
            var result = await Handle(payload);
            await session.SendMessage(result);
        }

        protected virtual async Task<byte[]> Handle(byte[] payload)
        {
            byte[] result = null;
            var readBufferContext = new ReadBufferContext(payload);
            var partitionedRequest = BasePartitionedRequest.Deserialize(readBufferContext);
            if (partitionedRequest == null) return null;
            if (partitionedRequest.Command == PartitionedCommands.TRANSFERED_REQUEST) result = await Handle(partitionedRequest as TransferedRequest);
            if (partitionedRequest.Command == PartitionedCommands.ADD_PARTITION_REQUEST) result = await Handle(partitionedRequest as AddDirectPartitionRequest);
            if (partitionedRequest.Command == PartitionedCommands.BROADCAST_REQUEST) result = await Handle(partitionedRequest as BroadcastRequest);
            return result;
        }

        private Task<byte[]> Handle(TransferedRequest request)
        {
            return _partitionCluster.Transfer(request, TokenSource.Token);
        }

        private async Task<byte[]> Handle(AddDirectPartitionRequest request)
        {
            await _partitionCluster.TryAddAndStart(request.PartitionKey);
            var result = new WriteBufferContext();
            PartitionPackageResultBuilder.AddPartition().SerializeEnvelope(result);
            return result.Buffer.ToArray();
        }

        private async Task<byte[]> Handle(BroadcastRequest request)
        {
            var broadcastResult = await _partitionCluster.Broadcast(request, TokenSource.Token);
            var result = new WriteBufferContext();
            PartitionPackageResultBuilder.Broadcast(broadcastResult).SerializeEnvelope(result);
            return result.Buffer.ToArray();
        }
    }
}
