using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class EventMeshCommands : BaseEnumeration
    {
        /// <summary>
        /// Heartbeat request sent to the server.
        /// </summary>
        public static EventMeshCommands HEARTBEAT_REQUEST = new EventMeshCommands(0, "HEARTBEAT_REQUEST");
        /// <summary>
        /// Heartbeat response replied by the server.
        /// </summary>
        public static EventMeshCommands HEARTBEAT_RESPONSE = new EventMeshCommands(1, "HEARTBEAT_RESPONSE");
        /// <summary>
        /// Request sent to add a VPN.
        /// </summary>
        public static EventMeshCommands ADD_VPN_REQUEST = new EventMeshCommands(2, "ADD_VPN_REQUEST");
        /// <summary>
        /// Response received when a VPN is created.
        /// </summary>
        public static EventMeshCommands ADD_VPN_RESPONSE = new EventMeshCommands(3, "ADD_VPN_RESPONSE");
        /// <summary>
        /// Request send to get all the VPNs
        /// </summary>
        public static EventMeshCommands GET_ALL_VPN_REQUEST = new EventMeshCommands(4, "GET_ALL_VPN_REQUEST");
        /// <summary>
        /// Response received when all VPNs are returned.
        /// </summary>
        public static EventMeshCommands GET_ALL_VPN_RESPONSE = new EventMeshCommands(5, "GET_ALL_VPN_RESPONSE");
        /// <summary>
        /// Request send to add a client.
        /// </summary>
        public static EventMeshCommands ADD_CLIENT_REQUEST = new EventMeshCommands(6, "ADD_CLIENT_REQUEST");
        /// <summary>
        /// Response received when a client is created.
        /// </summary>
        public static EventMeshCommands ADD_CLIENT_RESPONSE = new EventMeshCommands(7, "ADD_CLIENT_RESPONSE");
        /// <summary>
        /// Request send to get all the clients.
        /// </summary>
        public static EventMeshCommands GET_ALL_CLIENT_REQUEST = new EventMeshCommands(8, "GET_ALL_CLIENT_REQUEST");
        /// <summary>
        /// Response received when all clients are returned.
        /// </summary>
        public static EventMeshCommands GET_ALL_CLIENT_RESPONSE = new EventMeshCommands(9, "GET_ALL_CLIENT_RESPONSE");
        /// <summary>
        /// Request send to publish a message.
        /// </summary>
        public static EventMeshCommands PUBLISH_MESSAGE_REQUEST = new EventMeshCommands(10, "PUBLISH_MESSAGE_REQUEST");
        /// <summary>
        /// Response received when a message is published.
        /// </summary>
        public static EventMeshCommands PUBLISH_MESSAGE_RESPONSE = new EventMeshCommands(11, "PUBLISH_MESSAGE_RESPONSE");
        /// <summary>
        /// Request sent to create a queue.
        /// </summary>
        public static EventMeshCommands ADD_QUEUE_REQUEST = new EventMeshCommands(12, "ADD_QUEUE_REQUEST");
        /// <summary>
        /// Response sent when a queue is added.
        /// </summary>
        public static EventMeshCommands ADD_QUEUE_RESPONSE = new EventMeshCommands(13, "ADD_QUEUE_RESPONSE");
        /// <summary>
        /// Request sent to create a session.
        /// </summary>
        public static EventMeshCommands HELLO_REQUEST = new EventMeshCommands(14, "HELLO_REQUEST");
        /// <summary>
        /// Response sent when a session is created
        /// </summary>
        public static EventMeshCommands HELLO_RESPONSE = new EventMeshCommands(15, "HELLO_RESPONSE");
        /// <summary>
        /// Request sent to read a message on a specific location.
        /// </summary>
        public static EventMeshCommands READ_MESSAGE_REQUEST = new EventMeshCommands(16, "READ_MESSAGE_REQUEST");
        /// <summary>
        /// Response sent when a message is received.
        /// </summary>
        public static EventMeshCommands READ_MESSAGE_RESPONSE = new EventMeshCommands(17, "READ_MESSAGE_RESPONSE");
        /// <summary>
        /// Request sent to get a client.
        /// </summary>
        public static EventMeshCommands GET_CLIENT_REQUEST = new EventMeshCommands(18, "GET_CLIENT_REQUEST");
        /// <summary>
        /// Response sent when a client is received.
        /// </summary>
        public static EventMeshCommands GET_CLIENT_RESPONSE = new EventMeshCommands(19, "GET_CLIENT_RESPONSE");
        /// <summary>
        /// Request sent to search sessions.
        /// </summary>
        public static EventMeshCommands SEARCH_SESSIONS_REQUEST = new EventMeshCommands(20, "SEARCH_SESSIONS_REQUEST");
        /// <summary>
        /// Reponse sent when sessions are received.
        /// </summary>
        public static EventMeshCommands SEARCH_SESSIONS_RESPONSE = new EventMeshCommands(21, "SEARCH_SESSIONS_RESPONSE");
        /// <summary>
        /// Request sent to search queues.
        /// </summary>
        public static EventMeshCommands SEARCH_QUEUES_REQUEST = new EventMeshCommands(22, "SEARCH_QUEUES_REQUEST");
        /// <summary>
        /// Reponse sent when queues are received.
        /// </summary>
        public static EventMeshCommands SEARCH_QUEUES_RESPONSE = new EventMeshCommands(23, "SEARCH_QUEUES_RESPONSE");
        /// <summary>
        /// Request sent to find VPNS by name.
        /// </summary>
        public static EventMeshCommands FIND_VPNS_BY_NAME_REQUEST = new EventMeshCommands(24, "FIND_VPNS_BY_NAME_REQUEST");
        /// <summary>
        /// Response sent when VPNS are found.
        /// </summary>
        public static EventMeshCommands FIND_VPNS_BY_NAME_RESPONSE = new EventMeshCommands(25, "FIND_VPNS_BY_NAME_RESPONSE");
        /// <summary>
        /// Request sent to find clients by name.
        /// </summary>
        public static EventMeshCommands FIND_CLIENTS_BY_NAME_REQUEST = new EventMeshCommands(26, "FIND_CLIENTS_BY_NAME_REQUEST");
        /// <summary>
        /// Response returned when clients are found.
        /// </summary>
        public static EventMeshCommands FIND_CLIENTS_BY_NAME_RESPONSE = new EventMeshCommands(27, "FIND_CLIENTS_BY_NAME_RESPONSE");
        /// <summary>
        /// Request sent to find queues by name.
        /// </summary>
        public static EventMeshCommands FIND_QUEUES_BY_NAME_REQUEST = new EventMeshCommands(28, "FIND_QUEUES_BY_NAME_REQUEST");
        /// <summary>
        /// Response returned when queues are found.
        /// </summary>
        public static EventMeshCommands FIND_QUEUES_BY_NAME_RESPONSE = new EventMeshCommands(29, "FIND_QUEUES_BY_NAME_RESPONSE");
        /// <summary>
        /// Request sent to get the partition.
        /// </summary>
        public static EventMeshCommands GET_PARTITION_REQUEST = new EventMeshCommands(30, "GET_PARTITION_REQUEST");
        /// <summary>
        /// Result sent when partition is returned.
        /// </summary>
        public static EventMeshCommands GET_PARTITION_RESPONSE = new EventMeshCommands(31, "GET_PARTITION_RESPONSE");
        /// <summary>
        /// Request sent to add an event definition.
        /// </summary>
        public static EventMeshCommands ADD_EVENT_DEFINITION_REQUEST = new EventMeshCommands(32, "ADD_EVENT_DEFINITION_REQUEST");
        /// <summary>
        /// Result returned when event definition is added.
        /// </summary>
        public static EventMeshCommands ADD_EVENT_DEFINITION_RESPONSE = new EventMeshCommands(33, "ADD_EVENT_DEFINITION_RESPONSE");
        /// <summary>
        /// Request sent to get an event definition.
        /// </summary>
        public static EventMeshCommands GET_EVENT_DEFINITION_REQUEST = new EventMeshCommands(34, "GET_EVENT_DEFINITION_REQUEST");
        /// <summary>
        /// Result returned when event definition is returned.
        /// </summary>
        public static EventMeshCommands GET_EVENT_DEFINITION_RESULT = new EventMeshCommands(35, "GET_EVENT_DEFINITION_RESULT");
        /// <summary>
        /// Request sent to update an event definition.
        /// </summary>
        public static EventMeshCommands UPDATE_EVENT_DEFINITION_REQUEST = new EventMeshCommands(36, "UPDATE_EVENT_DEFINITION_REQUEST");
        /// <summary>
        /// Request returned when event definition is updated.
        /// </summary>
        public static EventMeshCommands UPDATE_EVENT_DEFINITION_RESULT = new EventMeshCommands(37, "UPDATE_EVENT_DEFINITION_RESULT");
        /// <summary>
        /// Request sent to remove an application domain link.
        /// </summary>
        public static EventMeshCommands REMOVE_LINK_APPLICATION_DOMAIN_REQUEST = new EventMeshCommands(38, "REMOVE_LINK_APPLICATION_DOMAIN_REQUEST");
        /// <summary>
        /// Request returned when application domain link is removed.
        /// </summary>
        public static EventMeshCommands REMOVE_LINK_APPLICATION_DOMAIN_RESULT = new EventMeshCommands(39, "REMOVE_LINK_APPLICATION_DOMAIN_RESULT");
        /// <summary>
        /// Request sent to add an application domain.
        /// </summary>
        public static EventMeshCommands ADD_APPLICATION_DOMAIN_REQUEST = new EventMeshCommands(40, "ADD_APPLICATION_DOMAIN_REQUEST");
        /// <summary>
        /// Result returned when an application domain is added.
        /// </summary>
        public static EventMeshCommands ADD_APPLICATION_DOMAIN_RESULT = new EventMeshCommands(41, "ADD_APPLICATION_DOMAIN_RESULT");
        /// <summary>
        /// Request sent to search application domains.
        /// </summary>
        public static EventMeshCommands GET_ALL_APPLICATION_DOMAINS_REQUEST = new EventMeshCommands(42, "GET_ALL_APPLICATION_DOMAINS_REQUEST");
        /// <summary>
        /// Result returned when application domains is returned.
        /// </summary>
        public static EventMeshCommands GET_ALL_APPLICATION_DOMAINS_RESULT = new EventMeshCommands(43, "GET_ALL_APPLICATION_DOMAINS_RESULT");
        /// <summary>
        /// Request sent to add the link application domain.
        /// </summary>
        public static EventMeshCommands ADD_LINK_APPLICATION_DOMAIN_REQUEST = new EventMeshCommands(44, "ADD_LINK_APPLICATION_DOMAIN_REQUEST");
        /// <summary>
        /// Result returned when the link application domain is added.
        /// </summary>
        public static EventMeshCommands ADD_LINK_APPLICATION_DOMAIN_RESULT = new EventMeshCommands(45, "ADD_LINK_APPLICATION_DOMAIN_RESULT");
        /// <summary>
        /// Request sent to update the coordinates of the application domain.
        /// </summary>
        public static EventMeshCommands UPDATE_APPLICATION_DOMAIN_COORDINATES_REQUEST = new EventMeshCommands(46, "UPDATE_APPLICATION_DOMAIN_COORDINATES");
        /// <summary>
        /// Result returned when the coordinates of the application domain are updated.
        /// </summary>
        public static EventMeshCommands UPDATE_APPLICATION_DOMAIN_COORDINATES_RESULT = new EventMeshCommands(47, "UPDATE_APPLICATION_DOMAIN_COORDINATES_RESULT");
        /// <summary>
        /// Request sent to get the application domain.
        /// </summary>
        public static EventMeshCommands GET_APPLICATION_DOMAIN_REQUEST = new EventMeshCommands(48, "GET_APPLICATION_DOMAIN_REQUEST");
        /// <summary>
        /// Result returned when the application domain is returned.
        /// </summary>
        public static EventMeshCommands GET_APPLICATION_DOMAIN_RESULT = new EventMeshCommands(49, "GET_APPLICATION_DOMAIN_RESULT");
        /// <summary>
        /// Request sent to add an element in the application domain.
        /// </summary>
        public static EventMeshCommands ADD_ELEMENT_APPLICATION_DOMAIN_REQUEST = new EventMeshCommands(50, "ADD_ELEMENT_APPLICATION_DOMAIN_REQUEST");
        /// <summary>
        /// Result sent when the application is added into the application domain.
        /// </summary>
        public static EventMeshCommands ADD_ELEMENT_APPLICATION_DOMAIN_RESULT = new EventMeshCommands(51, "ADD_ELEMENT_APPLICATION_DOMAIN_RESULT");
        /// <summary>
        /// Request sent to remove an element in the application domain.
        /// </summary>
        public static EventMeshCommands REMOVE_ELEMENT_APPLICATION_DOMAIN_REQUEST = new EventMeshCommands(52, "REMOVE_ELEMENT_APPLICATION_DOMAIN_REQUEST");
        /// <summary>
        /// Result sent when the application is removed from the application domain.
        /// </summary>
        public static EventMeshCommands REMOVE_ELEMENT_APPLICATION_DOMAIN_RESULT = new EventMeshCommands(53, "REMOVE_ELEMENT_APPLICATION_DOMAIN_RESULT");
        /// <summary>
        /// Request sent to get event definitions.
        /// </summary>
        public static EventMeshCommands GET_ALL_EVENT_DEFS_REQUEST = new EventMeshCommands(54, "GET_ALL_EVENT_DEFS_REQUEST");
        /// <summary>
        /// Result returned when event definitions are returned.
        /// </summary>
        public static EventMeshCommands GET_ALL_EVENT_DEFS_RESULT = new EventMeshCommands(55, "GET_ALL_EVENT_DEFS_RESULT");
        /// <summary>
        /// Request sent to get the ASYNCAPI definition.
        /// </summary>
        public static EventMeshCommands GET_ASYNC_API_REQUEST = new EventMeshCommands(56, "GET_ASYNC_API_REQUEST");
        /// <summary>
        /// Result returned when the ASYNCAPI definition is returned.
        /// </summary>
        public static EventMeshCommands GET_ASYNC_API_RESULT = new EventMeshCommands(57, "GET_ASYNC_API_RESULT");
        /// <summary>
        /// Request sent to add a subscription.
        /// </summary>
        public static EventMeshCommands ADD_SUBSCRIPTION_REQUEST = new EventMeshCommands(58, "ADD_SUBSCRIPTION_REQUEST");
        /// <summary>
        /// Result returned when subscription is added.
        /// </summary>
        public static EventMeshCommands ADD_SUBSCRIPTION_RESULT = new EventMeshCommands(59, "ADD_SUBSCRIPTION_RESULT");

        protected EventMeshCommands(int code)
        {
            Init<EventMeshCommands>(code);
        }

        protected EventMeshCommands(int code, string name) : base(code, name) { }

        public static EventMeshCommands Deserialize(ReadBufferContext context)
        {
            return new EventMeshCommands(context.NextInt());
        }
    }
}
