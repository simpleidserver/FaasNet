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
        /// Add a bridge.
        /// </summary>
        public static Commands ADD_BRIDGE_REQUEST = new Commands(6, "ADD_BRIDGE_REQUEST");
        /// <summary>
        /// Server reply add bridge response.
        /// </summary>
        public static Commands ADD_BRIDGE_RESPONSE = new Commands(7, "ADD_BRIDGE_RESPONSE");
        /// <summary>
        /// Send disconnect request to close the session.
        /// </summary>
        public static Commands DISCONNECT_REQUEST = new Commands(8, "DISCONNECT_REQUEST");
        /// <summary>
        /// Reply disconnect response.
        /// </summary>
        public static Commands DISCONNECT_RESPONSE = new Commands(9, "DISCONNECT_RESPONSE");
        /// <summary>
        /// Client send message request to the server.
        /// </summary>
        public static Commands PUBLISH_MESSAGE_REQUEST = new Commands(10, "PUBLISH_MESSAGE_REQUEST");
        /// <summary>
        /// Server reply message response to the client.
        /// </summary>
        public static Commands PUBLISH_MESSAGE_RESONSE = new Commands(11, "PUBLISH_MESSAGE_RESONSE");
        /// <summary>
        /// Get all VPN request.
        /// </summary>
        public static Commands GET_ALL_VPNS_REQUEST = new Commands(12, "GET_ALL_VPNS_REQUEST");
        /// <summary>
        /// Get all VPN response.
        /// </summary>
        public static Commands GET_ALL_VPNS_RESPONSE = new Commands(13, "GET_ALL_VPNS_RESPONSE");
        /// <summary>
        /// Request sent to create a client.
        /// </summary>
        public static Commands ADD_CLIENT_REQUEST = new Commands(14, "ADD_CLIENT_REQUEST");
        /// <summary>
        /// Result returned when a client is added;
        /// </summary>
        public static Commands ADD_CLIENT_RESPONSE = new Commands(15, "ADD_CLIENT_RESPONSE");
        /// <summary>
        /// Request sent to create a VPN.
        /// </summary>
        public static Commands ADD_VPN_REQUEST = new Commands(16, "ADD_VPN_REQUEST");
        /// <summary>
        /// Result returned when a VPN is added.
        /// </summary>
        public static Commands ADD_VPN_RESPONSE = new Commands(17, "ADD_VPN_RESPONSE");

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
