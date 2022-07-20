using FaasNet.Peer.Client;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class KademliaCommandTypes : BaseEnumeration
    {
        /// <summary>
        /// Used to verify that a node is still alive.
        /// </summary>
        public static KademliaCommandTypes PING_REQUEST = new KademliaCommandTypes(0, "PING_REQUEST");
        public static KademliaCommandTypes PING_RESULT = new KademliaCommandTypes(1, "PING_RESULT");
        /// <summary>
        /// Store a (key, value) pair in one node.
        /// </summary>
        public static KademliaCommandTypes STORE_REQUEST = new KademliaCommandTypes(2, "STORE_REQUEST");
        public static KademliaCommandTypes STORE_RESULT = new KademliaCommandTypes(3, "STORE_RESULT");
        /// <summary>
        /// The recipient of the request will return the k nodes in his own buckets that are the closest ones to the requested key.
        /// </summary>
        public static KademliaCommandTypes FIND_NODE_REQUEST = new KademliaCommandTypes(4, "FIND_NODE_REQUEST");
        public static KademliaCommandTypes FIND_NODE_RESULT = new KademliaCommandTypes(5, "FIND_NODE_RESULT");
        /// <summary>
        /// Same as FIND_NODE, but if the recipient of the request has the requested key in its store, it will return the corresponding value.
        /// </summary>
        public static KademliaCommandTypes FIND_VALUE_REQUEST = new KademliaCommandTypes(6, "FIND_VALUE_REQUEST");
        public static KademliaCommandTypes FIND_VALUE_RESULT = new KademliaCommandTypes(7, "FIND_VALUE_RESULT");
        /// <summary>
        /// Join the network.
        /// </summary>
        public static KademliaCommandTypes JOIN_REQUEST = new KademliaCommandTypes(8, "JOIN_REQUEST");
        public static KademliaCommandTypes JOIN_RESULT = new KademliaCommandTypes(9, "JOIN_RESULT");
        public KademliaCommandTypes(int code)
        {
            Init<KademliaCommandTypes>(code);
        }

        public KademliaCommandTypes(int code, string description) : base(code, description)
        {
        }

        public static KademliaCommandTypes Deserialize(ReadBufferContext context)
        {
            return new KademliaCommandTypes(context.NextInt());
        }
    }
}
