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
