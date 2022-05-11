using System;
using System.Linq;
using System.Reflection;

namespace FaasNet.EventMesh.Client.Messages
{
    public class Commands : IEquatable<Commands>
    {
        /// <summary>
        /// Client send heartbeat request to server.
        /// </summary>
        public static Commands HEARTBEAT_REQUEST = new Commands(0, "HEARTBEAT_REQUEST");
        /// <summary>
        /// Server reply heartbeat response to client.
        /// </summary>
        public static Commands HEARTBEAT_RESPONSE = new Commands(1, "HEARTBEAT_RESPONSE");
        /// <summary>
        /// Client send connect request to server.
        /// </summary>
        public static Commands HELLO_REQUEST = new Commands(2, "HELLO_REQUEST");
        /// <summary>
        /// Server reply connect response to client.
        /// </summary>
        public static Commands HELLO_RESPONSE = new Commands(3, "HELLO_RESPONSE");
        /// <summary>
        /// Client send subscribe request to server.
        /// </summary>
        public static Commands SUBSCRIBE_REQUEST = new Commands(4, "SUBSCRIBE_REQUEST");
        /// <summary>
        /// Server reply subscribe response to client.
        /// </summary>
        public static Commands SUBSCRIBE_RESPONSE = new Commands(5, "SUBSCRIBE_RESPONSE");
        /// <summary>
        /// Server push async message to client.
        /// </summary>
        public static Commands ASYNC_MESSAGE_TO_CLIENT = new Commands(6, "ASYNC_MESSAGE_TO_CLIENT");
        /// <summary>
        /// Client reply ack of async msg to server.
        /// </summary>
        public static Commands ASYNC_MESSAGE_TO_CLIENT_ACK = new Commands(7, "ASYNC_MESSAGE_TO_CLIENT_ACK");
        /// <summary>
        /// Add a bridge.
        /// </summary>
        public static Commands ADD_BRIDGE_REQUEST = new Commands(8, "ADD_BRIDGE_REQUEST");
        /// <summary>
        /// Server reply add bridge response.
        /// </summary>
        public static Commands ADD_BRIDGE_RESPONSE = new Commands(9, "ADD_BRIDGE_RESPONSE");
        /// <summary>
        /// Server sends async message to a server.
        /// </summary>
        public static Commands ASYNC_MESSAGE_TO_SERVER = new Commands(10, "ASYNC_MESSAGE_TO_SERVER");
        /// <summary>
        /// Send disconnect request to close the session.
        /// </summary>
        public static Commands DISCONNECT_REQUEST = new Commands(11, "DISCONNECT_REQUEST");
        /// <summary>
        /// Reply disconnect response.
        /// </summary>
        public static Commands DISCONNECT_RESPONSE = new Commands(12, "DISCONNECT_RESPONSE");
        public static Commands ASYNC_MESSAGE_TO_CLIENT_ACK_RESPONSE = new Commands(13, "ASYNC_MESSAGE_TO_CLIENT_ACK_RESPONSE");
        /// <summary>
        /// Client send message request to the server.
        /// </summary>
        public static Commands PUBLISH_MESSAGE_REQUEST = new Commands(14, "PUBLISH_MESSAGE_REQUEST");
        /// <summary>
        /// Server reply message response to the client.
        /// </summary>
        public static Commands PUBLISH_MESSAGE_RESONSE = new Commands(15, "PUBLISH_MESSAGE_RESONSE");
        /// <summary>
        /// Get all VPN request.
        /// </summary>
        public static Commands GET_ALL_VPNS_REQUEST = new Commands(16, "GET_ALL_VPNS_REQUEST");
        /// <summary>
        /// Get all VPN response.
        /// </summary>
        public static Commands GET_ALL_VPNS_RESPONSE = new Commands(17, "GET_ALL_VPNS_RESPONSE");
        /// <summary>
        /// Request sent to create a client.
        /// </summary>
        public static Commands ADD_CLIENT_REQUEST = new Commands(18, "ADD_CLIENT_REQUEST");
        /// <summary>
        /// Result returned when a client is added;
        /// </summary>
        public static Commands ADD_CLIENT_RESPONSE = new Commands(19, "ADD_CLIENT_RESPONSE");

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
