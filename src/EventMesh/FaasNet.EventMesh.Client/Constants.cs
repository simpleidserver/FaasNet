using FaasNet.EventMesh.Client.Messages;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client
{
    public class Constants
    {
        public const int DefaultPort = 4000;
        public const string DefaultUrl = "localhost";
        public const string DefaultVpn = "default";
        public const string DefaultIPAddress = "127.0.0.1";

        public static Dictionary<Commands, Commands> MappingRequestToResponse = new Dictionary<Commands, Commands>
        {
            { Commands.GET_ALL_BRIDGE_VPN_REQUEST, Commands.GET_ALL_BRIDGE_VPN_RESPONSE },
            { Commands.ADD_BRIDGE_REQUEST, Commands.ADD_BRIDGE_RESPONSE },
            { Commands.DISCONNECT_REQUEST, Commands.DISCONNECT_RESPONSE },
            { Commands.HEARTBEAT_REQUEST, Commands.HEARTBEAT_RESPONSE },
            { Commands.HELLO_REQUEST, Commands.HELLO_RESPONSE },
            { Commands.SUBSCRIBE_REQUEST, Commands.SUBSCRIBE_RESPONSE },
            { Commands.PUBLISH_MESSAGE_REQUEST, Commands.PUBLISH_MESSAGE_RESONSE },
            { Commands.GET_ALL_VPNS_REQUEST, Commands.GET_ALL_VPNS_RESPONSE },
            { Commands.ADD_VPN_REQUEST, Commands.ADD_VPN_RESPONSE },
            { Commands.ADD_CLIENT_REQUEST, Commands.ADD_CLIENT_RESPONSE }
        };
    }
}
