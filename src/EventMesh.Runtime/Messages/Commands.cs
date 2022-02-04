using System;

namespace EventMesh.Runtime.Messages
{
    public class Commands : IEquatable<Commands>
    {
        /// <summary>
        /// Client send heartbeat request to server.
        /// </summary>
        public static Commands HEARTBEAT_REQUEST = new Commands(0);
        /// <summary>
        /// Server reply heartbeat response to client.
        /// </summary>
        public static Commands HEARTBEAT_RESPONSE = new Commands(1);
        /// <summary>
        /// Client send connect request to server.
        /// </summary>
        public static Commands HELLO_REQUEST = new Commands(2);
        /// <summary>
        /// Server reply connect response to client.
        /// </summary>
        public static Commands HELLO_RESPONSE = new Commands(3);
        /// <summary>
        /// Client send subscribe request to server.
        /// </summary>
        public static Commands SUBSCRIBE_REQUEST = new Commands(8);
        /// <summary>
        /// Server reply subscribe response to client.
        /// </summary>
        public static Commands SUBSCRIBE_RESPONSE = new Commands(9);
        /// <summary>
        /// Server push async message to client.
        /// </summary>
        public static Commands ASYNC_MESSAGE_TO_CLIENT = new Commands(10);
        /// <summary>
        /// Client reply ack of async msg to server.
        /// </summary>
        public static Commands ASYNC_MESSAGE_TO_CLIENT_ACK = new Commands(11);

        private Commands(int code)
        {
            Code = code;
        }

        public int Code { get; private set; }

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
            if (a == null || b == null)
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
    }
}
