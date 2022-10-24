using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.StateMachines.Client;
using FaasNet.EventMesh.StateMachines.EventDefinition;
using FaasNet.EventMesh.StateMachines.Queue;
using FaasNet.EventMesh.StateMachines.Session;
using FaasNet.EventMesh.StateMachines.Vpn;
using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode : PartitionedNodeHost
    {
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly PartitionedNodeOptions _options;
        private readonly EventMeshOptions _eventMeshOptions;

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
            if (packageRequest.Command == EventMeshCommands.GET_CLIENT_REQUEST) packageResult = await Handle(packageRequest as GetClientRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.SEARCH_SESSIONS_REQUEST) packageResult = await Handle(packageRequest as SearchSessionsRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.SEARCH_QUEUES_REQUEST) packageResult = await Handle(packageRequest as SearchQueuesRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.FIND_VPNS_BY_NAME_REQUEST) packageResult = await Handle(packageRequest as FindVpnsByNameRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.FIND_QUEUES_BY_NAME_REQUEST) packageResult = await Handle(packageRequest as FindQueuesByNameRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.FIND_CLIENTS_BY_NAME_REQUEST) packageResult = await Handle(packageRequest as FindClientsByNameRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.GET_PARTITION_REQUEST) packageResult = await Handle(packageRequest as GetPartitionRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.ADD_EVENT_DEFINITION_REQUEST) packageResult = await Handle(packageRequest as AddEventDefinitionRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.GET_EVENT_DEFINITION_REQUEST) packageResult = await Handle(packageRequest as GetEventDefinitionRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.UPDATE_EVENT_DEFINITION_REQUEST) packageResult = await Handle(packageRequest as UpdateEventDefinitionRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.REMOVE_LINK_APPLICATION_DOMAIN_REQUEST) packageResult = await Handle(packageRequest as RemoveLinkApplicationDomainRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.ADD_APPLICATION_DOMAIN_REQUEST) packageResult = await Handle(packageRequest as AddApplicationDomainRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.GET_ALL_APPLICATION_DOMAINS_REQUEST) packageResult = await Handle(packageRequest as GetAllApplicationDomainsRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.ADD_LINK_APPLICATION_DOMAIN_REQUEST) packageResult = await Handle(packageRequest as AddLinkApplicationDomainRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.UPDATE_APPLICATION_DOMAIN_COORDINATES_REQUEST) packageResult = await Handle(packageRequest as UpdateApplicationDomainCoordinatesRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.GET_APPLICATION_DOMAIN_REQUEST) packageResult = await Handle(packageRequest as GetApplicationDomainRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.ADD_ELEMENT_APPLICATION_DOMAIN_REQUEST) packageResult = await Handle(packageRequest as AddElementApplicationDomainRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.REMOVE_ELEMENT_APPLICATION_DOMAIN_REQUEST) packageResult = await Handle(packageRequest as RemoveElementApplicationDomainRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.GET_ALL_EVENT_DEFS_REQUEST) packageResult = await Handle(packageRequest as GetAllEventDefsRequest, TokenSource.Token);
            if (packageRequest.Command == EventMeshCommands.GET_ASYNC_API_REQUEST) packageResult = await Handle(packageRequest as GetAsyncApiRequest, TokenSource.Token);
            var writeBufferContext = new WriteBufferContext();
            packageResult.SerializeEnvelope(writeBufferContext);
            return writeBufferContext.Buffer.ToArray();
        }

        private async void SeedPartitions()
        {
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = PartitionNames.VPN_PARTITION_KEY, Port = _options.StartPeerPort, StateMachineType = typeof(VpnStateMachine) });
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = PartitionNames.CLIENT_PARTITION_KEY, Port = _options.StartPeerPort + 1, StateMachineType = typeof(ClientStateMachine) });
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = PartitionNames.SESSION_PARTITION_KEY, Port = _options.StartPeerPort + 2, StateMachineType = typeof(SessionStateMachine) });
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = PartitionNames.QUEUE_PARTITION_KEY, Port = _options.StartPeerPort + 3, StateMachineType = typeof(QueueStateMachine) });
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = PartitionNames.EVENTDEFINITION_PARTITION_KEY, Port = _options.StartPeerPort + 4, StateMachineType = typeof(EventDefinitionStateMachine) });
            await PartitionPeerStore.Add(new DirectPartitionPeer { PartitionKey = PartitionNames.APPLICATION_DOMAIN, Port = _options.StartPeerPort + 5, StateMachineType = typeof(ApplicationDomainStateMachine) });
        }

        private async Task<AppendEntryResult> Send(string partitionKey, ICommand command, CancellationToken cancellationToken)
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.AppendEntry(CommandSerializer.Serialize(command)).SerializeEnvelope(writeBufferContext);
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

        private async Task<T> Query<T>(string partitionKey, IQuery query, CancellationToken cancellationToken) where T : IQueryResult
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.Query(query).SerializeEnvelope(writeBufferContext);
            var content = writeBufferContext.Buffer.ToArray();
            var transferedResult = await PartitionCluster.Transfer(new TransferedRequest
            {
                PartitionKey = partitionKey,
                Content = content
            }, cancellationToken);
            var readBufferContext = new ReadBufferContext(transferedResult);
            var stateMachineResult = BaseConsensusPackage.Deserialize(readBufferContext) as QueryResult;
            return (T)stateMachineResult.Result;
        }

        private async Task<GetPeerStateResult> GetPeerState(string partitionKey, CancellationToken cancellationToken)
        {
            var writeBufferContext = new WriteBufferContext();
            ConsensusPackageRequestBuilder.GetPeerState().SerializeEnvelope(writeBufferContext);
            var content = writeBufferContext.Buffer.ToArray();
            var transferedResult = await PartitionCluster.Transfer(new TransferedRequest
            {
                PartitionKey = partitionKey,
                Content = content
            }, cancellationToken);
            var readBufferContext = new ReadBufferContext(transferedResult);
            return BaseConsensusPackage.Deserialize(readBufferContext) as GetPeerStateResult;
        }
    }
}
