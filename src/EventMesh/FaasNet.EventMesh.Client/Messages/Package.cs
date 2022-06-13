
using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class Package
    {
        private const string MAGIC_CODE = "EventMesh";
        private const string PROTOCOL_VERSION = "0000";

        public Header Header { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            context.WriteString(MAGIC_CODE);
            context.WriteString(PROTOCOL_VERSION);
            Header.Serialize(context);
        }

        public static Package Deserialize(ReadBufferContext context)
        {
            context.NextString();
            context.NextString();
            var header = Header.Deserialize(context);
            if (Commands.HELLO_REQUEST == header.Command)
            {
                var result = new HelloRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.SUBSCRIBE_REQUEST == header.Command)
            {
                var result = new SubscriptionRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.ADD_CLIENT_REQUEST == header.Command)
            {
                var result = new AddClientRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.ADD_CLIENT_RESPONSE == header.Command) return new AddClientResponse { Header = header };
            if (Commands.SUBSCRIBE_RESPONSE == header.Command)
            {
                var result = new SubscriptionResult
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.DISCONNECT_REQUEST == header.Command)
            {
                var result = new DisconnectRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }
            
            if (Commands.HELLO_RESPONSE == header.Command)
            {
                var result = new HelloResponse
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if (Commands.PUBLISH_MESSAGE_REQUEST == header.Command)
            {
                var result = new PublishMessageRequest
                {
                    Header= header
                };
                result.Extract(context);
                return result;
            }

            if(Commands.GET_ALL_VPNS_RESPONSE == header.Command)
            {
                var result = new GetAllVpnResponse
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }


            if (Commands.ADD_VPN_REQUEST == header.Command)
            {
                var result = new AddVpnRequest
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }
            if (Commands.READ_NEXT_MESSAGE_REQUEST == header.Command)
            {
                var result = new ReadNextMessageRequest { Header = header };
                result.Extract(context);
                return result;
            }

            if (Commands.READ_NEXT_MESSAGE_RESPONSE == header.Command)
            {
                var result = new ReadNextMessageResponse { Header = header };
                result.Extract(context);
                return result;
            }

            if (Commands.READ_TOPIC_MESSAGE_REQUEST == header.Command)
            {
                var result = new ReadMessageTopicRequest { Header = header };
                result.Extract(context);
                return result;
            }

            if (Commands.READ_TOPIC_MESSAGE_RESPONSE == header.Command)
            {
                var result = new ReadMessageTopicResponse { Header = header };
                result.Extract(context);
                return result;
            }

            if (Commands.ADD_BRIDGE_REQUEST == header.Command)
            {
                var result = new AddBridgeRequest { Header = header };
                result.Extract(context);
                return result;
            }

            if (Commands.GET_ALL_BRIDGE_VPN_RESPONSE == header.Command)
            {
                var result = new GetAllBridgeResponse { Header = header };
                result.Extract(context);
                return result;
            }

            if(Commands.GET_ALL_PLUGINS_RESPONSE == header.Command)
            {
                var result = new GetAllPluginsResponse { Header = header };
                result.Extract(context);
                return result;
            }

            if(Commands.ENABLE_PLUGIN_REQUEST == header.Command)
            {
                var result = new EnablePluginRequest { Header = header };
                result.Extract(context);
                return result;
            }

            if (Commands.DISABLE_PLUGIN_REQUEST == header.Command)
            {
                var result = new DisablePluginRequest { Header = header };
                result.Extract(context);
                return result;
            }

            if(Commands.GET_PLUGIN_CONFIGURATION_REQUEST == header.Command)
            {
                var result = new GetPluginConfigurationRequest { Header = header };
                result.Extract(context);
                return result;
            }

            if(Commands.GET_PLUGIN_CONFIGURATION_RESPONSE == header.Command)
            {
                var result = new GetPluginConfigurationResponse { Header = header };
                if (header.Status == HeaderStatus.SUCCESS) result.Extract(context);
                return result;
            }

            if (Commands.UPDATE_PLUGIN_CONFIGURATION_REQUEST == header.Command)
            {
                var result = new UpdatePluginConfigurationRequest { Header = header };
                if (header.Status == HeaderStatus.SUCCESS) result.Extract(context);
                return result;
            }

            return new Package
            {
                Header = header
            };
        }
    }
}
