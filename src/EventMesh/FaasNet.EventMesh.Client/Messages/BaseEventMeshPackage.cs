using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public abstract class BaseEventMeshPackage : BasePeerPackage
    {
        public static string MAGIC_CODE = "EventMesh";
        public static string VERSION = "0000";
        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => VERSION;
        public abstract EventMeshCommands Command { get; }
        public string Seq { get; private set; }

        public BaseEventMeshPackage(string seq)
        {
            Seq = seq;
        }

        public override void SerializeBody(WriteBufferContext context)
        {
            Command.Serialize(context);
            context.WriteString(Seq);
            SerializeAction(context);
        }

        protected abstract void SerializeAction(WriteBufferContext context);

        public static BaseEventMeshPackage Deserialize(ReadBufferContext context, bool ignoreEnvelope = false)
        {
            if (!ignoreEnvelope)
            {
                var magicCode = context.NextString();
                var version = context.NextString();
                if (magicCode != MAGIC_CODE || version != VERSION) return null;
            }

            var cmd = EventMeshCommands.Deserialize(context);
            var seq = context.NextString();
            if (cmd == EventMeshCommands.HEARTBEAT_REQUEST) return new PingRequest(seq);
            if (cmd == EventMeshCommands.HEARTBEAT_RESPONSE) return new PingResult(seq);
            if (cmd == EventMeshCommands.ADD_VPN_REQUEST) return new AddVpnRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_VPN_RESPONSE) return new AddVpnResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_VPN_REQUEST) return new GetAllVpnRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_VPN_RESPONSE) return new GetAllVpnResult(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_CLIENT_REQUEST) return new AddClientRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_CLIENT_RESPONSE) return new AddClientResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_CLIENT_REQUEST) return new GetAllClientRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_CLIENT_RESPONSE) return new GetAllClientResult(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_QUEUE_REQUEST) return new AddQueueRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_QUEUE_RESPONSE) return new AddQueueResponse(seq).Extract(context);
            if (cmd == EventMeshCommands.PUBLISH_MESSAGE_REQUEST) return new PublishMessageRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.PUBLISH_MESSAGE_RESPONSE) return new PublishMessageResult(seq).Extract(context);
            if (cmd == EventMeshCommands.HELLO_REQUEST) return new HelloRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.HELLO_RESPONSE) return new HelloResult(seq).Extract(context);
            if (cmd == EventMeshCommands.READ_MESSAGE_REQUEST) return new ReadMessageRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.READ_MESSAGE_RESPONSE) return new ReadMessageResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_CLIENT_REQUEST) return new GetClientRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_CLIENT_RESPONSE) return new GetClientResult(seq).Extract(context);
            if (cmd == EventMeshCommands.SEARCH_SESSIONS_REQUEST) return new SearchSessionsRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.SEARCH_SESSIONS_RESPONSE) return new SearchSessionsResult(seq).Extract(context);
            if (cmd == EventMeshCommands.SEARCH_QUEUES_REQUEST) return new SearchQueuesRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.SEARCH_QUEUES_RESPONSE) return new SearchQueuesResult(seq).Extract(context);
            if (cmd == EventMeshCommands.FIND_VPNS_BY_NAME_REQUEST) return new FindVpnsByNameRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.FIND_VPNS_BY_NAME_RESPONSE) return new FindVpnsByNameResult(seq).Extract(context);
            if (cmd == EventMeshCommands.FIND_CLIENTS_BY_NAME_REQUEST) return new FindClientsByNameRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.FIND_CLIENTS_BY_NAME_RESPONSE) return new FindClientsByNameResult(seq).Extract(context);
            if (cmd == EventMeshCommands.FIND_QUEUES_BY_NAME_REQUEST) return new FindQueuesByNameRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.FIND_QUEUES_BY_NAME_RESPONSE) return new FindQueuesByNameResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_PARTITION_REQUEST) return new GetPartitionRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_PARTITION_RESPONSE) return new GetPartitionResult(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_EVENT_DEFINITION_REQUEST) return new AddEventDefinitionRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_EVENT_DEFINITION_RESPONSE) return new AddEventDefinitionResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_EVENT_DEFINITION_REQUEST) return new GetEventDefinitionRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_EVENT_DEFINITION_RESULT) return new GetEventDefinitionResult(seq).Extract(context);
            if (cmd == EventMeshCommands.UPDATE_EVENT_DEFINITION_REQUEST) return new UpdateEventDefinitionRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.UPDATE_EVENT_DEFINITION_RESULT) return new UpdateEventDefinitionResult(seq).Extract(context);
            if (cmd == EventMeshCommands.REMOVE_LINK_APPLICATION_DOMAIN_REQUEST) return new RemoveLinkApplicationDomainRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.REMOVE_LINK_APPLICATION_DOMAIN_RESULT) return new RemoveLinkApplicationDomainResult(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_APPLICATION_DOMAIN_REQUEST) return new AddApplicationDomainRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_APPLICATION_DOMAIN_RESULT) return new AddApplicationDomainResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_APPLICATION_DOMAINS_REQUEST) return new GetAllApplicationDomainsRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_ALL_APPLICATION_DOMAINS_RESULT) return new GetAllApplicationDomainsResult(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_LINK_APPLICATION_DOMAIN_REQUEST) return new AddLinkApplicationDomainRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_LINK_APPLICATION_DOMAIN_RESULT) return new AddLinkApplicationDomainResult(seq).Extract(context);
            if (cmd == EventMeshCommands.UPDATE_APPLICATION_DOMAIN_COORDINATES_REQUEST) return new UpdateApplicationDomainCoordinatesRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.UPDATE_APPLICATION_DOMAIN_COORDINATES_REQUEST) return new UpdateApplicationDomainCoordinatesResult(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_APPLICATION_DOMAIN_REQUEST) return new GetApplicationDomainRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.GET_APPLICATION_DOMAIN_RESULT) return new GetApplicationDomainResult(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_ELEMENT_APPLICATION_DOMAIN_REQUEST) return new AddElementApplicationDomainRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.ADD_ELEMENT_APPLICATION_DOMAIN_RESULT) return new AddElementApplicationDomainResult(seq).Extract(context);
            if (cmd == EventMeshCommands.REMOVE_ELEMENT_APPLICATION_DOMAIN_REQUEST) return new RemoveElementApplicationDomainRequest(seq).Extract(context);
            if (cmd == EventMeshCommands.REMOVE_ELEMENT_APPLICATION_DOMAIN_RESULT) return new RemoveElementApplicationDomainResult(seq).Extract(context);
            return null;
        }
    }
}
