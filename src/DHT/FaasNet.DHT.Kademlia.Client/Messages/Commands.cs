using System.Linq;
using System.Reflection;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class Commands
    {
        /// <summary>
        /// Used to verify that a node is still alive.
        /// </summary>
        public static Commands PING_REQUEST = new Commands(0, "PING_REQUEST");
        public static Commands PING_RESULT = new Commands(1, "PING_RESULT");
        /// <summary>
        /// Store a (key, value) pair in one node.
        /// </summary>
        public static Commands STORE_REQUEST = new Commands(2, "STORE_REQUEST");
        public static Commands STORE_RESULT = new Commands(3, "STORE_RESULT");
        /// <summary>
        /// The recipient of the request will return the k nodes in his own buckets that are the closest ones to the requested key.
        /// </summary>
        public static Commands FIND_NODE_REQUEST = new Commands(4, "FIND_NODE_REQUEST");
        public static Commands FIND_NODE_RESULT = new Commands(5, "FIND_NODE_RESULT");
        /// <summary>
        /// Same as FIND_NODE, but if the recipient of the request has the requested key in its store, it will return the corresponding value.
        /// </summary>
        public static Commands FIND_VALUE_REQUEST = new Commands(6, "FIND_VALUE_REQUEST");
        public static Commands FIND_VALUE_RESULT = new Commands(7, "FIND_VALUE_RESULT");
        /// <summary>
        /// Join the network.
        /// </summary>
        public static Commands JOIN_REQUEST = new Commands(8, "JOIN_REQUEST");
        public static Commands JOIN_RESULT = new Commands(9, "JOIN_RESULT");

        private Commands(int code)
        {
            var cmdType = typeof(Commands).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Single(f => f.FieldType == typeof(Commands) && ((Commands)f.GetValue(null)).Code == code);
            var name = ((Commands)cmdType.GetValue(null)).Name;
            Code = code;
            Name = name;
        }

        private Commands(int code, string name)
        {
            Code = code;
            Name = name;
        }

        public int Code { get; private set; }
        public string Name { get; private set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(Code);
        }

        public static bool operator ==(Commands a, Commands b)
        {
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Commands a, Commands b)
        {
            if ((object)a == null || (object)b == null)
            {
                return true;
            }

            return !a.Equals(b);
        }

        public static Commands Deserialize(ReadBufferContext context)
        {
            return new Commands(context.NextInt());
        }

        public bool Equals(Commands other)
        {
            if (other == null)
            {
                return false;
            }

            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as Commands;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Code;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
