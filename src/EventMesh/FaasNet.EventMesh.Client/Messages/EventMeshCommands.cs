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
