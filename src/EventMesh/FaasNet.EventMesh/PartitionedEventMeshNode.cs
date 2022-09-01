using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using FaasNet.Peer.Transports;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Client.StateMachines;
using FaasNet.RaftConsensus.Core.StateMachines;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode : PartitionedNodeHost
    {
        private readonly IPartitionPeerStore _partitionPeerStore;
        private readonly PartitionedNodeOptions _options;
        private const string VPN_PARTITION_KEY = "VPN";
        private const string CLIENT_PARTITION_KEY = "CLIENT";

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
            if (packageRequest.Command == EventMeshCommands.ADD_VPN_REQUEST) packageResult = await Handle(packageRequest as AddVpnRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.GET_ALL_VPN_REQUEST) packageResult = await Handle(packageRequest as GetAllVpnRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.ADD_CLIENT_REQUEST) packageResult = await Handle(packageRequest as AddClientRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.GET_ALL_CLIENT_REQUEST) packageResult = await Handle(packageRequest as GetAllClientRequest, TokenSource.Token);
            var writeBufferContext = new WriteBufferContext();
            packageResult.SerializeEnvelope(writeBufferContext);
            return writeBufferContext.Buffer.ToArray();
        }

        private async void SeedPartitions()
        {
            await _partitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = VPN_PARTITION_KEY, Port = _options.StartPeerPort, StateMachineType = typeof(VpnCollection) });
            await _partitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = CLIENT_PARTITION_KEY, Port = _options.StartPeerPort + 1, StateMachineType = typeof(ClientCollection) });
        }

        private async Task Send(string partitionKey, ICommand command, CancellationToken cancellationToken)
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.AppendEntry(CommandSerializer.Serialize(command)).SerializeEnvelope(writeBufferContext);
            var cmdBuffer = writeBufferContext.Buffer.ToArray();
            await PartitionCluster.Transfer(new TransferedRequest
            {
                Content = cmdBuffer,
                PartitionKey = partitionKey
            }, cancellationToken);
        }

        private async Task<T> GetStateMachine<T>(string partitionKey, CancellationToken cancellationToken) where T : IStateMachine
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.GetStateMachine().SerializeEnvelope(writeBufferContext);
            var content = writeBufferContext.Buffer.ToArray();
            var transferedResult = await PartitionCluster.Transfer(new TransferedRequest
            {
                PartitionKey = partitionKey,
                Content = content
            }, cancellationToken);
            var readBufferContext = new ReadBufferContext(transferedResult);
            var stateMachineResult = BaseConsensusPackage.Deserialize(readBufferContext) as GetStateMachineResult;
            return StateMachineSerializer.Deserialize<T>(stateMachineResult.StateMachine);
        }
    }
}
