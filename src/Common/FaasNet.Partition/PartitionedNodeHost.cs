using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public class PartitionedNodeHost : BasePeerHost
    {
        public static string PARTITION_KEY = "*";
        private readonly PeerOptions _peerOptions;
        private readonly IClusterStore _clusterStore;
        private readonly IPartitionPeerStore _partitionPeerStore;
        private readonly IPartitionCluster _partitionCluster;

        public PartitionedNodeHost(IOptions<PeerOptions> peerOptions, IClusterStore clusterStore, IPartitionPeerStore partitionPeerStore, IPartitionCluster partitionCluster, IServerTransport transport, IProtocolHandlerFactory protocolHandlerFactory, IEnumerable<ITimer> timers, ILogger<BasePeerHost> logger) : base(transport, protocolHandlerFactory, timers, logger)
        {
            _peerOptions = peerOptions.Value;
            _clusterStore = clusterStore;
            _partitionPeerStore = partitionPeerStore;
            _partitionCluster = partitionCluster;
        }

        protected IPartitionCluster PartitionCluster => _partitionCluster;
        protected IPartitionPeerStore PartitionPeerStore => _partitionPeerStore;
        protected IClusterStore ClusterStore => _clusterStore;
        protected PeerOptions PeerOptions => _peerOptions;

        protected override async Task Init(CancellationToken cancellationToken = default)
        {   
            await _clusterStore.SelfRegister(new ClusterPeer(_peerOptions.Url, _peerOptions.Port) { PartitionKey = PARTITION_KEY }, cancellationToken);
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
            if (partitionedRequest.Command == PartitionedCommands.REMOVE_PARTITION_REQUEST) result = await Handle(partitionedRequest as RemoveDirectPartitionRequest);
            if (partitionedRequest.Command == PartitionedCommands.BROADCAST_REQUEST) result = await Handle(partitionedRequest as BroadcastRequest);
            if (partitionedRequest.Command == PartitionedCommands.GET_ALL_NODES_REQUEST) result = await Handle(partitionedRequest as GetAllNodesRequest);
            return result;
        }

        private Task<byte[]> Handle(TransferedRequest request)
        {
            return _partitionCluster.Transfer(request, TokenSource.Token);
        }

        private async Task<byte[]> Handle(AddDirectPartitionRequest request)
        {
            var stateMachine = Type.GetType(request.StateMachineType);
            var status = AddDirectPartitionStatus.SUCCESS;
            if (!await _partitionCluster.TryAddAndStart(request.PartitionKey, stateMachine))
                status = AddDirectPartitionStatus.ERROR;
            var result = new WriteBufferContext();
            PartitionPackageResultBuilder.AddPartition(status).SerializeEnvelope(result);
            return result.Buffer.ToArray();
        }

        private async Task<byte[]> Handle(RemoveDirectPartitionRequest request)
        {
            var status = RemoveDirectPartitionStatus.SUCCESS;
            if (!await _partitionCluster.TryRemove(request.PartitionKey, TokenSource.Token))
                status = RemoveDirectPartitionStatus.UNKNOWN_PARTITION;
            var result = new WriteBufferContext();
            PartitionPackageResultBuilder.RemovePartition(status).SerializeEnvelope(result);
            return result.Buffer.ToArray();
        }

        private async Task<byte[]> Handle(BroadcastRequest request)
        {
            var broadcastResult = await _partitionCluster.Broadcast(request, TokenSource.Token);
            var result = new WriteBufferContext();
            PartitionPackageResultBuilder.Broadcast(broadcastResult).SerializeEnvelope(result);
            return result.Buffer.ToArray();
        }

        private async Task<byte[]> Handle(GetAllNodesRequest request)
        {
            var nodes = await _clusterStore.GetAllNodes(PARTITION_KEY, TokenSource.Token);
            var result = new WriteBufferContext();
            PartitionPackageResultBuilder.GetAllNodes(nodes.Select(n => new NodeResult
            {
                Id = n.Id,
                Port = n.Port,
                Url = n.Url
            }).ToList()).SerializeEnvelope(result);
            return result.Buffer.ToArray();
        }
    }
}
