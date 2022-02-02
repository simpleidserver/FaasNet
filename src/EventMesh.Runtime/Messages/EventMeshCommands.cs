using System;

namespace EventMesh.Runtime.Messages
{
    public class EventMeshCommands : IEquatable<EventMeshCommands>
    {
        /// <summary>
        /// Client send heartbeat request to server.
        /// </summary>
        public static EventMeshCommands HEARTBEAT_REQUEST = new EventMeshCommands(0);
        /// <summary>
        /// Server reply heartbeat response to client.
        /// </summary>
        public static EventMeshCommands HEARTBEAT_RESPONSE = new EventMeshCommands(1);

        private EventMeshCommands(int code)
        {
            Code = code;
        }

        public int Code { get; private set; }

        public void Serialize(EventMeshWriterBufferContext context)
        {
            context.WriteInteger(Code);
        }

        public static bool operator ==(EventMeshCommands a, EventMeshCommands b)
        {
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(EventMeshCommands a, EventMeshCommands b)
        {
            if (a == null || b == null)
            {
                return true;
            }

            return !a.Equals(b);
        }

        public static EventMeshCommands Deserialize(EventMeshReaderBufferContext context)
        {
            return new EventMeshCommands(context.NextInt());
        }

        public bool Equals(EventMeshCommands other)
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

            var other = obj as EventMeshCommands;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Code;
        }
    }
}
