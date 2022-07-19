using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class ChordCommandTypes : BaseEnumeration
    {
        /// <summary>
        /// Get dimension finger table.
        /// </summary>
        public static ChordCommandTypes GET_DIMENSION_FINGER_TABLE_REQUEST = new ChordCommandTypes(0, "GET_DIMENSION_FINGER_TABLE_REQUEST");
        /// <summary>
        /// Dimension of the finger table.
        /// </summary>
        public static ChordCommandTypes GET_DIMENSION_FINGER_TABLE_RESULT = new ChordCommandTypes(1, "GET_DIMENSION_FINGER_TABLE_RESULT");
        /// <summary>
        /// Join CHORD network.
        /// </summary>
        public static ChordCommandTypes JOIN_CHORD_NETWORK_REQUEST = new ChordCommandTypes(2, "JOIN_CHORD_NETWORK_REQUEST");
        public static ChordCommandTypes JOIN_CHORD_NETWORK_RESULT = new ChordCommandTypes(3, "JOIN_CHORD_NETWORK_RESULT");
        /// <summary>
        /// Find successor.
        /// </summary>
        public static ChordCommandTypes FIND_SUCCESSOR_REQUEST = new ChordCommandTypes(4, "FIND_SUCCESSOR_REQUEST");
        public static ChordCommandTypes FIND_SUCCESSOR_RESULT = new ChordCommandTypes(5, "FIND_SUCCESSOR_RESULT");
        /// <summary>
        /// Create a new Chord ring.
        /// </summary>
        public static ChordCommandTypes CREATE_REQUEST = new ChordCommandTypes(6, "CREATE_REQUEST");
        public static ChordCommandTypes CREATE_RESULT = new ChordCommandTypes(7, "CREATE_RESULT");
        /// <summary>
        /// Notify the existence of a node.
        /// </summary>
        public static ChordCommandTypes NOTIFY_REQUEST = new ChordCommandTypes(8, "NOTIFY_REQUEST");
        public static ChordCommandTypes NOTIFY_RESULT = new ChordCommandTypes(9, "NOTIFY_RESULT");
        /// <summary>
        /// Find predecessor request.
        /// </summary>
        public static ChordCommandTypes FIND_PREDECESSOR_REQUEST = new ChordCommandTypes(10, "FIND_PREDECESSOR_REQUEST");
        public static ChordCommandTypes FIND_PREDECESSOR_RESULT = new ChordCommandTypes(11, "FIND_PREDECESSOR_RESULT");
        /// <summary>
        /// Add key.
        /// </summary>
        public static ChordCommandTypes ADD_KEY_REQUEST = new ChordCommandTypes(12, "ADD_KEY_REQUEST");
        public static ChordCommandTypes ADD_KEY_RESULT = new ChordCommandTypes(13, "ADD_KEY_RESULT");
        /// <summary>
        /// Get key.
        /// </summary>
        public static ChordCommandTypes GET_KEY_REQUEST = new ChordCommandTypes(14, "GET_KEY_REQUEST");
        public static ChordCommandTypes GET_KEY_RESULT = new ChordCommandTypes(15, "GET_KEY_RESULT");
        public static ChordCommandTypes EMPTY_RESULT = new ChordCommandTypes(16, "EMPTY_RESULT");
        public ChordCommandTypes(int code)
        {
            Init<ChordCommandTypes>(code);
        }

        public ChordCommandTypes(int code, string description) : base(code, description)
        {
        }

        public static ChordCommandTypes Deserialize(ReadBufferContext context)
        {
            return new ChordCommandTypes(context.NextInt());
        }
    }
}
