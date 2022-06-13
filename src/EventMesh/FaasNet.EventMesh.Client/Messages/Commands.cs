using FaasNet.RaftConsensus.Client.Messages;
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
        /// Request sent to read the next message.
        /// </summary>
        public static Commands READ_NEXT_MESSAGE_REQUEST = new Commands(18, "READ_MESSAGE_REQUEST");
        /// <summary>
        /// Result returned when message is sent.
        /// </summary>
        public static Commands READ_NEXT_MESSAGE_RESPONSE = new Commands(19, "READ_MESSAGE_RESPONSE");
        /// <summary>
        /// Request sent to read topic message.
        /// </summary>
        public static Commands READ_TOPIC_MESSAGE_REQUEST = new Commands(20, "READ_TOPIC_MESSAGE_REQUEST");
        /// <summary>
        /// Result returned when message is sent.
        /// </summary>
        public static Commands READ_TOPIC_MESSAGE_RESPONSE = new Commands(21, "READ_TOPIC_MESSAGE_RESPONSE");
        /// <summary>
        /// Request sent to get all VPN bridge.
        /// </summary>
        public static Commands GET_ALL_BRIDGE_VPN_REQUEST = new Commands(22, "GET_ALL_BRIDGE_VPN_REQUEST");
        /// <summary>
        /// Result returned when VPN are returned.
        /// </summary>
        public static Commands GET_ALL_BRIDGE_VPN_RESPONSE = new Commands(23, "GET_ALL_BRIDGE_VPN_RESPONSE");
        /// <summary>
        /// Request sent to get all the plugins.
        /// </summary>
        public static Commands GET_ALL_PLUGINS_REQUEST = new Commands(24, "GET_ALL_PLUGINS_REQUEST");
        /// <summary>
        /// Results returned when plugins are returned.
        /// </summary>
        public static Commands GET_ALL_PLUGINS_RESPONSE = new Commands(25, "GET_ALL_PLUGINS_RESPONSE");
        /// <summary>
        /// Request sent to enable a plugin.
        /// </summary>
        public static Commands ENABLE_PLUGIN_REQUEST = new Commands(26, "ENABLE_PLUGIN_REQUEST");
        /// <summary>
        /// Result returned when plugin is enabled.
        /// </summary>
        public static Commands ENABLE_PLUGIN_RESPONSE = new Commands(27, "ENABLE_PLUGIN_RESPONSE");
        /// <summary>
        /// Request sent to disable a plugin.
        /// </summary>
        public static Commands DISABLE_PLUGIN_REQUEST = new Commands(28, "DISABLE_PLUGIN_REQUEST");
        /// <summary>
        /// Result returned when a plugin is disabled.
        /// </summary>
        public static Commands DISABLE_PLUGIN_RESPONSE = new Commands(29, "DISABLE_PLUGIN_RESPONSE");
        /// <summary>
        /// Request sent to get the plugin configuration.
        /// </summary>
        public static Commands GET_PLUGIN_CONFIGURATION_REQUEST = new Commands(30, "GET_PLUGIN_CONFIGURATION_REQUEST");
        /// <summary>
        /// Result returned to get the plugin configuration
        /// </summary>
        public static Commands GET_PLUGIN_CONFIGURATION_RESPONSE = new Commands(31, "GET_PLUGIN_CONFIGURATION_RESPONSE");
        /// <summary>
        /// Request sent to update the plugin configuration.
        /// </summary>
        public static Commands UPDATE_PLUGIN_CONFIGURATION_REQUEST = new Commands(32, "UPDATE_PLUGIN_CONFIGURATION_REQUEST");
        /// <summary>
        /// Result returned when the plugin configuration is updated.
        /// </summary>
        public static Commands UPDATE_PLUGIN_CONFIGURATION_RESPONSE = new Commands(33, "UPDATE_PLUGIN_CONFIGURATION_RESPONSE");


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
