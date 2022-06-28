using System.Linq;
using System.Reflection;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class Commands
    {
        /// <summary>
        /// Get dimension finger table.
        /// </summary>
        public static Commands GET_DIMENSION_FINGER_TABLE_REQUEST = new Commands(0, "GET_DIMENSION_FINGER_TABLE_REQUEST");
        /// <summary>
        /// Dimension of the finger table.
        /// </summary>
        public static Commands GET_DIMENSION_FINGER_TABLE_RESULT = new Commands(1, "GET_DIMENSION_FINGER_TABLE_RESULT");
        /// <summary>
        /// Join CHORD network.
        /// </summary>
        public static Commands JOIN_CHORD_NETWORK_REQUEST = new Commands(2, "JOIN_CHORD_NETWORK_REQUEST");
        public static Commands JOIN_CHORD_NETWORK_RESULT = new Commands(3, "JOIN_CHORD_NETWORK_RESULT");
        /// <summary>
        /// Find successor.
        /// </summary>
        public static Commands FIND_SUCCESSOR_REQUEST = new Commands(4, "FIND_SUCCESSOR_REQUEST");
        public static Commands FIND_SUCCESSOR_RESULT = new Commands(5, "FIND_SUCCESSOR_RESULT");
        /// <summary>
        /// Create a new Chord ring.
        /// </summary>
        public static Commands CREATE_REQUEST = new Commands(6, "CREATE_REQUEST");
        public static Commands CREATE_RESULT = new Commands(7, "CREATE_RESULT");
        /// <summary>
        /// Notify the existence of a node.
        /// </summary>
        public static Commands NOTIFY_REQUEST = new Commands(8, "NOTIFY_REQUEST");
        public static Commands NOTIFY_RESULT = new Commands(9, "NOTIFY_RESULT");
        /// <summary>
        /// Find predecessor request.
        /// </summary>
        public static Commands FIND_PREDECESSOR_REQUEST = new Commands(10, "FIND_PREDECESSOR_REQUEST");
        public static Commands FIND_PREDECESSOR_RESULT = new Commands(11, "FIND_PREDECESSOR_RESULT");

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
