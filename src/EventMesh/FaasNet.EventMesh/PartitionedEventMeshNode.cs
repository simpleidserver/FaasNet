using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Transports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode : PartitionedNodeHost
    {
        private readonly IPartitionPeerStore _partitionPeerStore;
        private readonly PartitionedNodeOptions _options;
        private const string VPN_PARTITION_KEY = "VPN";

        public PartitionedEventMeshNode(IPartitionPeerStore partitionPeerStore, IOptions<PartitionedNodeOptions> options, IPartitionCluster partitionCluster, IServerTransport transport, IProtocolHandlerFactory protocolHandlerFactory, IEnumerable<ITimer> timers, ILogger<BasePeerHost> logger) : base(partitionCluster, transport, protocolHandlerFactory, timers, logger)
        {
            _partitionPeerStore = partitionPeerStore;
            _options = options.Value;
            SeedPartitions();
        }

        protected override async Task<byte[]> Handle(byte[] payload)
        {
            var result = await base.Handle(payload);
            if (result != null) return result;
            var readBufferContext = new ReadBufferContext(payload);
            var packageRequest = BaseEventMeshPackage.Deserialize(readBufferContext);
            BaseEventMeshPackage packageResult = null;
            if (packageRequest.Command == EventMeshCommands.HEARTBEAT_REQUEST) packageResult = await Handle(packageRequest as PingRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.ADD_VPN_REQUEST) packageRequest = await Handle(packageRequest as AddVpnRequest, TokenSource.Token);
            var writeBufferContext = new WriteBufferContext();
            packageRequest.SerializeEnvelope(writeBufferContext);
            return writeBufferContext.Buffer.ToArray();
        }

        private async void SeedPartitions()
        {
            await _partitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = VPN_PARTITION_KEY, Port = _options.StartPeerPort, StateMachineType = typeof(VpnCollection) });
        }
    }
}
