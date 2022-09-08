using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Client.StateMachines;
using FaasNet.RaftConsensus.Core.StateMachines;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode : PartitionedNodeHost
    {
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly PartitionedNodeOptions _options;
        private readonly EventMeshOptions _eventMeshOptions;
        private const string VPN_PARTITION_KEY = "VPN";
        private const string CLIENT_PARTITION_KEY = "CLIENT";
        private const string SESSION_PARTITION_KEY = "SESSION";
        private const string QUEUE_PARTITION_KEY = "QUEUE";

        public PartitionedEventMeshNode(IPeerClientFactory peerClientFactory, IOptions<EventMeshOptions> eventMeshOptions, IOptions<PeerOptions> peerOptions, IClusterStore clusterStore, IPartitionPeerStore partitionPeerStore, IOptions<PartitionedNodeOptions> options, IPartitionCluster partitionCluster, IServerTransport transport, IProtocolHandlerFactory protocolHandlerFactory, IEnumerable<ITimer> timers, ILogger<BasePeerHost> logger) : base(peerOptions, clusterStore, partitionPeerStore, partitionCluster, transport, protocolHandlerFactory, timers, logger)
        {
            _peerClientFactory = peerClientFactory;
            _options = options.Value;
            _eventMeshOptions = eventMeshOptions.Value;
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
            if (packageRequest.Command == EventMeshCommands.ADD_QUEUE_REQUEST) packageResult = await Handle(packageRequest as AddQueueRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.PUBLISH_MESSAGE_REQUEST) packageResult = await Handle(packageRequest as PublishMessageRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.HELLO_REQUEST) packageResult = await Handle(packageRequest as HelloRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.READ_MESSAGE_REQUEST) packageResult = await Handle(packageRequest as ReadMessageRequest, TokenSource.Token);
            var writeBufferContext = new WriteBufferContext();
            packageResult.SerializeEnvelope(writeBufferContext);
            return writeBufferContext.Buffer.ToArray();
        }

        private async void SeedPartitions()
        {
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = VPN_PARTITION_KEY, Port = _options.StartPeerPort, StateMachineType = typeof(VpnStateMachine) });
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = CLIENT_PARTITION_KEY, Port = _options.StartPeerPort + 1, StateMachineType = typeof(ClientStateMachine) });
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = SESSION_PARTITION_KEY, Port = _options.StartPeerPort + 2, StateMachineType = typeof(SessionStateMachine) });
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = QUEUE_PARTITION_KEY, Port = _options.StartPeerPort + 3, StateMachineType = typeof(QueueStateMachine) });
        }

        private async Task<AppendEntryResult> Send(string partitionKey, string stateMachineId, ICommand command, CancellationToken cancellationToken)
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.AppendEntry(stateMachineId, CommandSerializer.Serialize(command)).SerializeEnvelope(writeBufferContext);
            var cmdBuffer = writeBufferContext.Buffer.ToArray();
            var transferedResult = await PartitionCluster.Transfer(new TransferedRequest
            {
                Content = cmdBuffer,
                PartitionKey = partitionKey
            }, cancellationToken);
            var readBufferContext = new ReadBufferContext(transferedResult);
            var result = BaseConsensusPackage.Deserialize(readBufferContext) as AppendEntryResult;
            return result;
        }

        private async Task<T> GetStateMachine<T>(string partitionKey, string stateMachineId, CancellationToken cancellationToken) where T : IStateMachine
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.GetStateMachine(stateMachineId).SerializeEnvelope(writeBufferContext);
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

        private async Task<T> ReadStateMachine<T>(string partitionKey, int offset, CancellationToken cancellationToken) where T : IStateMachine
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.ReadStateMachine(offset).SerializeEnvelope(writeBufferContext);
            var content = writeBufferContext.Buffer.ToArray();
            var transferedResult = await PartitionCluster.Transfer(new TransferedRequest
            {
                PartitionKey = partitionKey,
                Content = content
            }, cancellationToken);
            var readBufferContext = new ReadBufferContext(transferedResult);
            var stateMachineResult = BaseConsensusPackage.Deserialize(readBufferContext) as ReadStateMachineResult;
            return StateMachineSerializer.Deserialize<T>(stateMachineResult.StateMachine);
        }

        private async Task<IEnumerable<T>> GetAllStateMachines<T>(string partitionKey, CancellationToken cancellationToken) where T : IStateMachine
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.GetAllStateMachines().SerializeEnvelope(writeBufferContext);
            var content = writeBufferContext.Buffer.ToArray();
            var transferedResult = await PartitionCluster.Transfer(new TransferedRequest
            {
                PartitionKey = partitionKey,
                Content = content
            }, cancellationToken);
            var readBufferContext = new ReadBufferContext(transferedResult);
            var stateMachineResult = BaseConsensusPackage.Deserialize(readBufferContext) as GetAllStateMachinesResult;
            return stateMachineResult.States.Select(s => StateMachineSerializer.Deserialize<T>(s.StateMachine));
        }
    }
}
